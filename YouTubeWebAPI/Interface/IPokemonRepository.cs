using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface IPokemonRepository
    {
        ICollection<Pokemon> GetPokemons();
        Pokemon GetPokemon(int id);
        Pokemon GetPokemon(string name);
        decimal GetPokemonRating(int pokemonId);
        bool PokemonExist(int pokemonId);
        bool CreatePokemon(Pokemon pokemon);
        bool Save();
    }
}
