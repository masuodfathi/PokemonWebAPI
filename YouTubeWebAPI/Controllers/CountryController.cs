using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _country;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository country, IMapper mapper)
        {
            _country = country;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Country>))]
        public IActionResult GetCountries()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var countries = _country.GetCountries();
            return Ok(countries);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200,Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountry(int countryId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_country.CountryExsits(countryId))
                return NotFound();
            var country = _country.GetCountry(countryId);
            return Ok(country);
        }

        [HttpGet("country/owner/{ownerId}")]
        [ProducesResponseType(200,Type = typeof(Country))]
        [ProducesResponseType(400)]
        public IActionResult GetCountryOfAnOwner(int ownerId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var contries = _country.GetCountryByOwner(ownerId);
            return Ok(contries);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto country)
        {
            var con = _country.GetCountries().Where(c=>c.Name.Trim().ToUpper() ==  country.Name.Trim().ToUpper()).FirstOrDefault();
            if (con != null)
            {
                ModelState.AddModelError("", "The country already exist!");
                return BadRequest(ModelState);
            }
                
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var countryMap = _mapper.Map<Country>(country);
            if (!_country.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while saving...!");
                return StatusCode(500, ModelState);
            }
            return Ok("Country is created successfully");
        }
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdateCountry(CountryDto updatedCountry)
        {
            if(updatedCountry == null)
                return BadRequest(ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_country.CountryExsits(updatedCountry.Id))
            {
                ModelState.AddModelError("Error", "Not Found");
                return StatusCode(404, ModelState);
            }

            var updatedCountryMap = _mapper.Map<Country>(updatedCountry);
            if (!_country.UpdateCountry(updatedCountryMap))
            {
                ModelState.AddModelError("Server Error", "Something went wrong while saving...!");
                return StatusCode(500,ModelState);
            }

            return Ok("Country updated successfully");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_country.CountryExsits(countryId))
            {
                ModelState.AddModelError("Error", "Country does not exist!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_country.DeleteCountry(countryId))
            {
                ModelState.AddModelError("Server Error", "Somthing went wrong while deleting country...!");
                return StatusCode(500, ModelState);
            }
            return Ok("Country deleted!");
        }
    }
}
