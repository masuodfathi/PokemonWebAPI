using AutoMapper;
using YouTubeWebAPI.Data;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

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
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
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
            return _context.Pokemons.OrderBy(p => p.Id).ToList();
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

        public bool UpdatePokemon(Pokemon pokemon)
        {
            _context.Update(pokemon);
            return Save();
        }
    }
}
