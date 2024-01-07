using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface IOwnerRepository
    {
        ICollection<Owner> GetOwners();
        Owner GetOwner(string firstName);
        Owner GetOwner(int Id);
        ICollection<Owner> GetOwnerOfAPokemon(int pokeId);
        ICollection<Pokemon> GetPokemonByOwner(int ownerId);
        bool OwnerExist(int  ownerId);
    }
}
