using YouTubeWebAPI.Data;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Repository
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly DataContext _context;
        public OwnerRepository(DataContext context)
        {
            _context = context;
        }
        public Owner GetOwner(string firstName)
        {
            return _context.Owners.Where(o => o.FirstName == firstName).FirstOrDefault();
        }

        public Owner GetOwner(int id)
        {
            return _context.Owners.Where(o => o.Id == id).FirstOrDefault();
        }

        public ICollection<Owner> GetOwnerOfAPokemon(int pokeId)
        {
            return _context.PokemonOwners.Where(po => po.PokemonId == pokeId).Select(o => o.Owner).ToList();
        }

        public ICollection<Owner> GetOwners()
        {
            return _context.Owners.ToList();
        }

        public ICollection<Pokemon> GetPokemonByOwner(int ownerId)
        {
            return _context.PokemonOwners.Where(po => po.Owner.Id == ownerId).Select(p=>p.Pokemon).ToList();
        }

        public bool OwnerExist(int ownerId)
        {
            return _context.Owners.Any(o => o.Id == ownerId);
        }
    }
}
