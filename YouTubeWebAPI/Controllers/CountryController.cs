using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _country;
        public CountryController(ICountryRepository country)
        {
            _country = country;
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
    }
}
