using YouTubeWebAPI.Data;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;
        public PokemonRepository(DataContext dataContext) 
        {
            _context = dataContext;
        }

        public Pokemon GetPokemon(int id)
        {
            throw new NotImplementedException();
        }

        public Pokemon GetPokemon(string name)
        {
            throw new NotImplementedException();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            throw new NotImplementedException();
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p => p.Id).ToList();
        }

        public bool PokemonExist(int pokemonId)
        {
            throw new NotImplementedException();
        }
    }
}
