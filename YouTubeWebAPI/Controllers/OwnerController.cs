using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/{controller}")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        public OwnerController(IOwnerRepository ownerRepository)
        {
            _ownerRepository = ownerRepository;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwners()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var owners = _ownerRepository.GetOwners();
            return Ok(owners);
        }
        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int ownerId) 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if(!_ownerRepository.OwnerExist(ownerId))
                return NotFound();
            var owner = _ownerRepository.GetOwner(ownerId);
            return Ok(owner);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_ownerRepository.OwnerExist(ownerId))
                return NotFound();

            var pokemons = _ownerRepository.GetPokemonByOwner(ownerId);
            return Ok(pokemons);
        }
    }
}
