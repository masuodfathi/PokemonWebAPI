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
        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p => p.Id).ToList();
        }
    }
}
