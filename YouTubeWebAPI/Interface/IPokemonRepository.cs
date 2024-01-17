using YouTubeWebAPI.DTOs;
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
        bool CreatePokemon(PokemonDto pokemonDto);
        bool UpdatePokemon(Pokemon pokemon);
        bool DeletePokemon(int pokemonId);
        bool Save();
    }
}
