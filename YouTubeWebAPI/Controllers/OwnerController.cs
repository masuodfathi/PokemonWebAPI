using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public OwnerController(IOwnerRepository ownerRepository,ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
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
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromBody]OwnerDto newOwner)
        {
            var owner  = _ownerRepository.GetOwners()
                .Where(o => o.FirstName.Trim().ToUpper() == newOwner.FirstName.Trim().ToUpper()).FirstOrDefault();

            if (owner != null)
                ModelState.AddModelError("Error", "This firstname already exist!");

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(newOwner);
            ownerMap.Country = _countryRepository.GetCountry(newOwner.CountryId);
            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("Server Error", "Somthing went wrong while saving...!");
                return StatusCode(500, ModelState);
            }
            return Ok("Owner has created successfully!");
        }

        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdateOwner(OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ownerRepository.OwnerExist(updatedOwner.Id))
            {
                ModelState.AddModelError("Error", "Not Found");
                return StatusCode(404, ModelState);
            }

            var updatedOwnerMap = _mapper.Map<Owner>(updatedOwner);
            updatedOwnerMap.Country = _countryRepository.GetCountry(updatedOwner.CountryId);
            if (!_ownerRepository.UpdateOwner(updatedOwnerMap))
            {
                ModelState.AddModelError("Server Error", "Something went wrong while saving...!");
                return StatusCode(500, ModelState);
            }

            return Ok("Owner updated successfully");
        }
    }
}
