using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository, IOwnerRepository ownerRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _ownerRepository = ownerRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _pokemonRepository.GetPokemons();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }
        [HttpGet("{pokeId}")]
        [ProducesResponseType(200,Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExist(pokeId))
            {
                return NotFound();
            }
            var pokemon = _pokemonRepository.GetPokemon(pokeId);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }
        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200,Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExist(pokeId))
                return NotFound();
            if(!ModelState.IsValid)
                return BadRequest();
            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromBody]PokemonDto newPokemon)
        {
            if (newPokemon == null)
                return BadRequest(ModelState);

            var pokemon = _pokemonRepository.GetPokemons().Where(p => p.Name.Trim().ToUpper() == newPokemon.Name.Trim().ToUpper()).SingleOrDefault();

            if(pokemon != null)
            {
                ModelState.AddModelError("Error", "The name already exist!");
                return StatusCode(422,ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_pokemonRepository.CreatePokemon(newPokemon))
            {
                ModelState.AddModelError("Error", "Somthing went wrong while saving...!");
                return StatusCode(500,ModelState);
            }

            return Ok("Pokemon has created successfully!");
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdatePokemon(PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExist(updatedPokemon.Id))
            {
                ModelState.AddModelError("Error", "Not Found");
                return StatusCode(404, ModelState);
            }

            var updatedPokemonMap = _mapper.Map<Pokemon>(updatedPokemon);
            if (!_pokemonRepository.UpdatePokemon(updatedPokemonMap))
            {
                ModelState.AddModelError("Server Error", "Something went wrong while saving...!");
                return StatusCode(500, ModelState);
            }

            return Ok("Pokemon updated successfully");
        }
    }
}
