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

            // Convert database entities into the public response shape.
            var response = pokemons.Select(p => new PokemonResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                BirthDate = p.BirthDate,
                Categories = p.PokemonCategories.Select(pc => new PokemonCategoryResponseDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToList(),
                Owners = p.PokemonOwners.Select(po => new PokemonOwnerResponseDto
                {
                    Id = po.Owner.Id,
                    FirstName = po.Owner.FirstName,
                    LastName = po.Owner.LastName
                }).ToList()
            }).ToList();
            return Ok(response);
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

            var response = new PokemonResponseDto
            {
                Id = pokemon.Id,
                Name = pokemon.Name,
                BirthDate = pokemon.BirthDate,
                Categories = pokemon.PokemonCategories.Select(pc => new PokemonCategoryResponseDto
                {
                    Id = pc.Category.Id,
                    Name = pc.Category.Name
                }).ToList(),
                Owners = pokemon.PokemonOwners.Select(po => new PokemonOwnerResponseDto
                {
                    Id = po.Owner.Id,
                    FirstName = po.Owner.FirstName,
                    LastName = po.Owner.LastName
                }).ToList()
            };
            return Ok(response);
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
        public IActionResult UpdatePokemon([FromBody] UpdatePokemonDto updatedPokemon)
        {
            // Check input validation results.
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            //// Let the repository validate and synchronize the relationships.
            var result = _pokemonRepository.UpdatePokemon(updatedPokemon);

            switch (result)
            {
                case PokemonUpdateResult.NotFound:
                    return NotFound(new { error = "Pokemon not found." });
                case PokemonUpdateResult.InvalidInput:
                    return BadRequest(new { error = "Invalid input data." });
                case PokemonUpdateResult.InvalidOwner:
                    return BadRequest(new { error = "One or more owner IDs are invalid." });
                case PokemonUpdateResult.InvalidCategory:
                    return BadRequest(new { error = "One or more category IDs are invalid." });
                case PokemonUpdateResult.Updated:
                    return Ok(new { message = "Pokemon updated successfully." });
                default:
                    return StatusCode(500, new { error = "An unexpected error occurred." });
                
            }
            return Ok("Pokemon updated successfully");
        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExist(pokemonId))
            {
                ModelState.AddModelError("Error", "Pokemon does not exist!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_pokemonRepository.DeletePokemon(pokemonId))
            {
                ModelState.AddModelError("Server Error", "Somthing went wrong while deleting pokemon...!");
                return StatusCode(500, ModelState);
            }
            return Ok("Pokemon deleted!");
        }
    }
}
