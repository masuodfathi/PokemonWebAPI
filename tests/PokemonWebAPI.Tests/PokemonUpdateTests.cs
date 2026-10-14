using AutoMapper;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Helper;
using YouTubeWebAPI.Models;
using YouTubeWebAPI.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using YouTubeWebAPI.Data;
using Xunit;

namespace PokemonWebAPI.Tests;

public class PokemonUpdateTests
{
    private static DataContext CreateContext(SqliteConnection connection)
    {
        // Use the supplied connection for the test database.
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;

        return new DataContext(options);
    }
    [Fact]
    public void Update_with_whitespace_name_returns_invalid_input()
    {
        // Arrange: Prepare dependencies and invalid inputs.
        var mapper = new MapperConfiguration(config => 
            config.AddProfile<MappingProfiles>()).CreateMapper();

        var repository = new PokemonRepository(null!, mapper);
        

        var request = new UpdatePokemonDto
        {
            Id = 1,
            Name = "  ",
            BirthDate = DateTime.Now,
            OwnerIds = new List<int> { 1, 2 },
            CategoryIds = new List<int> { 1, 2 }
        };

        var result = repository.UpdatePokemon(request);

        // Assert: Verify that the result indicates invalid input.
        Assert.Equal(PokemonUpdateResult.InvalidInput, result);
    }

    [Fact]
    public void Update_without_owners_returns_invalid_input()
    {
        // Arrange: prepare dependencies and invalid inputs.
        var mapper = new MapperConfiguration(config =>
            config.AddProfile<MappingProfiles>())
            .CreateMapper();

        var repository = new PokemonRepository(null!, mapper);

        var request = new UpdatePokemonDto
        {
            Id = 1,
            Name = "Pikachu",
            BirthDate = DateTime.Now,
            OwnerIds = new List<int>(), // No owners provided
            CategoryIds = new List<int> { 1, 2 }
        };

        // Act: Call the UpdatePokemon method with the invalid request.
        var result = repository.UpdatePokemon(request);
        
        // Assert: Verify that the result indicates invalid input.
        Assert.Equal(PokemonUpdateResult.InvalidInput, result);

    }

    [Fact]
    public void Update_replaces_owners_and_persists_changes()
    {
        // Arrange: keep one in-memory database alive for the whole test.
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var birthDate = new DateTime(2000, 1, 1);

        // Prepare the initial data using a separate context.
        using (var setupContext = CreateContext(connection))
        {
            setupContext.Database.EnsureCreated();

            var country = new Country
            {
                Id = 1,
                Name = "Test Country"
            };

            setupContext.Owners.AddRange(
                new Owner
                {
                    Id = 1,
                    FirstName = "Ash",
                    LastName = "Ketchum",
                    Gym = "Gym A",
                    Country = country
                },
                new Owner
                {
                    Id = 2,
                    FirstName = "Misty",
                    LastName = "Waterflower",
                    Gym = "Gym B",
                    Country = country
                },
                new Owner
                {
                    Id = 3,
                    FirstName = "Brock",
                    LastName = "Harrison",
                    Gym = "Gym C",
                    Country = country
                }
            );

            setupContext.Categories.Add(new Category
            {
                Id = 1,
                Name = "Electric"
            });

            setupContext.Pokemons.Add(new Pokemon
            {
                Id = 1,
                Name = "Pikachu",
                BirthDate = birthDate,

                PokemonOwners = new List<PokemonOwner>
                {
                    new PokemonOwner { OwnerId = 1 },
                    new PokemonOwner { OwnerId = 2 }
                },

                PokemonCategories = new List<PokemonCategory>
                {
                    new PokemonCategory { CategoryID = 1 }
                }
            });

            setupContext.SaveChanges();
        }

        var mapper = new MapperConfiguration(config =>
            config.AddProfile<MappingProfiles>())
            .CreateMapper();

        var request = new UpdatePokemonDto
        {
            Id = 1,
            Name = "Pikachu",
            BirthDate = birthDate,
            OwnerIds = new List<int> { 2, 3 },
            CategoryIds = new List<int> { 1 }
        };

        // Act: update the Pokemon using a fresh context.
        using (var updateContext = CreateContext(connection))
        {
            var repository = new PokemonRepository(updateContext, mapper);

            var result = repository.UpdatePokemon(request);

            Assert.Equal(PokemonUpdateResult.Updated, result);
        }

        // Assert: read the persisted relationships using another fresh context.
        using (var verifyContext = CreateContext(connection))
        {
            var ownerIds = verifyContext.PokemonOwners
                .Where(link => link.PokemonId == 1)
                .OrderBy(link => link.OwnerId)
                .Select(link => link.OwnerId)
                .ToArray();

            Assert.Equal(new[] { 2, 3 }, ownerIds);

            // Updating relationships must not delete the owners themselves.
            Assert.Equal(3, verifyContext.Owners.Count());
        }
    }
}