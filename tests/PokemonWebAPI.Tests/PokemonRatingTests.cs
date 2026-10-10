using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using YouTubeWebAPI.Controllers;
using YouTubeWebAPI.Data;
using YouTubeWebAPI.Helper;
using YouTubeWebAPI.Models;
using YouTubeWebAPI.Repository;
using Xunit;

namespace PokemonWebAPI.Tests;

public sealed class PokemonRatingTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DataContext _context;
    private readonly PokemonRepository _repository;
    private readonly PokemonController _controller;

    public PokemonRatingTests()
    {
        // Keep a relational, in-memory database open for the lifetime of each test.
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection).Options;
        _context = new DataContext(options);
        _context.Database.EnsureCreated();
        var mapper = new MapperConfiguration(config =>
            config.AddProfile<MappingProfiles>()).CreateMapper();
        _repository = new PokemonRepository(_context, mapper);
        _controller = new PokemonController(_repository, new OwnerRepository(_context), mapper);
    }

    [Fact]
    public void Rating_without_reviews_is_zero()
    {
        var pokemon = AddPokemon("Pikachu");
        Assert.Equal(0m, _repository.GetPokemonRating(pokemon.Id));
    }

    [Theory]
    [InlineData(new[] { 5 }, 5.0)]
    [InlineData(new[] { 1, 2 }, 1.5)]
    [InlineData(new[] { 1, 3, 5 }, 3.0)]
    public void Rating_is_the_average_of_only_the_requested_pokemon(int[] ratings, double expected)
    {
        var pokemon = AddPokemon("Pikachu", ratings);
        AddPokemon("Squirtle", 1, 1, 1);
        Assert.Equal((decimal)expected, _repository.GetPokemonRating(pokemon.Id));
    }

    [Fact]
    public void Rating_action_returns_ok_with_zero_when_no_reviews_exist()
    {
        var pokemon = AddPokemon("Pikachu");
        var result = Assert.IsType<OkObjectResult>(_controller.GetPokemonRating(pokemon.Id));
        Assert.Equal(0m, Assert.IsType<decimal>(result.Value));
    }

    [Fact]
    public void Rating_action_returns_not_found_for_an_unknown_pokemon()
    {
        Assert.IsType<NotFoundResult>(_controller.GetPokemonRating(999));
    }

    private Pokemon AddPokemon(string name, params int[] ratings)
    {
        var pokemon = new Pokemon
        {
            Name = name,
            BirthDate = new DateTime(2000, 1, 1),
            PokemonCategories = new List<PokemonCategory>(),
            PokemonOwners = new List<PokemonOwner>(),
            Reviews = ratings.Select(rating => new Review
            {
                Title = "Test review",
                Text = "Test review content",
                Rating = rating,
                Reviewer = new Reviewer { FirstName = "Test", LastName = "Reviewer" }
            }).ToList()
        };
        _context.Pokemons.Add(pokemon);
        _context.SaveChanges();
        return pokemon;
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }
}
