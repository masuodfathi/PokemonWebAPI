using AutoMapper;
using YouTubeWebAPI.Data;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace YouTubeWebAPI.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public PokemonRepository(DataContext dataContext, IMapper mapper) 
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public bool CreatePokemon(PokemonDto pokemonDto)
        {
            var owner = _context.Owners.Where(x => x.Id == pokemonDto.OwnerId).FirstOrDefault();
            var category = _context.Categories.Where(x => x.Id == pokemonDto.CategoryId).FirstOrDefault();
            var pokemon = _mapper.Map<Pokemon>(pokemonDto);

            var pokemonOwner = new PokemonOwner()
            {
                Owner = owner,
                Pokemon = pokemon
            };

            _context.PokemonOwners.Add(pokemonOwner);

            var pokemonCategory = new PokemonCategory()
            {
                Category = category,
                Pokemon = pokemon
            };

            _context.PokemonCategories.Add(pokemonCategory);
            _context.Pokemons.Add(pokemon);

            return Save();
        }

        public bool DeletePokemon(int pokemonId)
        {
            var pokemonToDelete = _context.Pokemons.Find(pokemonId);
            if (pokemonToDelete != null)
            {
                _context.Pokemons.Remove(pokemonToDelete);
                return Save();
            }
            return false;
        }

        public Pokemon GetPokemon(int id)
        {
            // Load the requested Pokemon and its related data.
            return _context.Pokemons
                .Include(p => p.PokemonOwners)
                    .ThenInclude(po => po.Owner)
                .Include(p => p.PokemonCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefault(p => p.Id == id);
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name)
                .Include(p => p.PokemonOwners)
                    .ThenInclude(po => po.Owner)
                .Include(p => p.PokemonCategories)
                    .ThenInclude(pc => pc.Category)
                .FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            // A nullable average returns null for an empty set instead of dividing by zero.
            var average = _context.Reviews
                .Where(review => review.Pokemon.Id == pokemonId)
                .Average(review => (double?)review.Rating);

            return average.HasValue ? (decimal)average.Value : 0m;
        }

        public ICollection<Pokemon> GetPokemons()
        {
            // Load Pokemon records together with their owners and categories.
            return _context.Pokemons
                .Include(p => p.PokemonOwners)
                    .ThenInclude(po => po.Owner)
                .Include(p => p.PokemonCategories)
                    .ThenInclude(pc => pc.Category)
                .OrderBy(p => p.Id)
                .ToList();
        }

        public bool PokemonExist(int pokemonId)
        {
            return _context.Pokemons.Any(p => p.Id == pokemonId);
        }

        public bool Save()
        {
            try
            {
                var save = _context.SaveChanges();
                return save > 0 ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public PokemonUpdateResult UpdatePokemon(UpdatePokemonDto dto)
        {
            // Validate input before modifying tracked entities.
            if (dto.Id <= 0 ||
                string.IsNullOrWhiteSpace(dto.Name) ||
                dto.OwnerIds == null || dto.OwnerIds.Count == 0 ||
                dto.CategoryIds == null || dto.CategoryIds.Count == 0)
            {
                return PokemonUpdateResult.InvalidInput;
            }

            // Remove duplicate IDs from the requested relationships.
            var ownerIds = dto.OwnerIds.ToHashSet();
            var categoryIds = dto.CategoryIds.ToHashSet();

            // Load the existing Pokemon and its relationship records.
            var pokemon = _context.Pokemons
                .Include(p => p.PokemonOwners)
                .Include(p => p.PokemonCategories)
                .FirstOrDefault(p => p.Id == dto.Id);

            if (pokemon == null)
                return PokemonUpdateResult.NotFound;

            // Confirm that every requested owner exists.
            var ownerCount = _context.Owners
                .Count(owner => ownerIds.Contains(owner.Id));

            if (ownerCount != ownerIds.Count)
                return PokemonUpdateResult.InvalidOwner;

            // Confirm that every requested category exists.
            var categoryCount = _context.Categories
                .Count(category => categoryIds.Contains(category.Id));

            if (categoryCount != categoryIds.Count)
                return PokemonUpdateResult.InvalidCategory;

            // Update the Pokemon's own properties.
            pokemon.Name = dto.Name.Trim();
            pokemon.BirthDate = dto.BirthDate;

            // Remove owner relationships that are no longer requested.
            var ownersToRemove = pokemon.PokemonOwners
                .Where(link => !ownerIds.Contains(link.OwnerId))
                .ToList();

            _context.PokemonOwners.RemoveRange(ownersToRemove);

            // Add only owner relationships that do not already exist.
            var existingOwnerIds = pokemon.PokemonOwners
                .Select(link => link.OwnerId)
                .ToHashSet();

            foreach (var ownerId in ownerIds.Except(existingOwnerIds))
            {
                pokemon.PokemonOwners.Add(new PokemonOwner
                {
                    PokemonId = pokemon.Id,
                    OwnerId = ownerId
                });
            }

            // Remove category relationships that are no longer requested.
            var categoriesToRemove = pokemon.PokemonCategories
                .Where(link => !categoryIds.Contains(link.CategoryID))
                .ToList();

            _context.PokemonCategories.RemoveRange(categoriesToRemove);

            // Add only category relationships that do not already exist.
            var existingCategoryIds = pokemon.PokemonCategories
                .Select(link => link.CategoryID)
                .ToHashSet();

            foreach (var categoryId in categoryIds.Except(existingCategoryIds))
            {
                pokemon.PokemonCategories.Add(new PokemonCategory
                {
                    PokemonId = pokemon.Id,
                    CategoryID = categoryId
                });
            }

            // Persist all changes together; unchanged input is also successful.
            _context.SaveChanges();

            return PokemonUpdateResult.Updated;
        }
    }
}
