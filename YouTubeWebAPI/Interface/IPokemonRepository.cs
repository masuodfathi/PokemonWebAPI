using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
    }
}
