using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryConteroller : Controller
    {
        private readonly ICategoryRepository _category;
        public CategoryConteroller(ICategoryRepository category)
        {
            _category = category;
        }
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategories()
        {
            var categories = _category.GetCategories();
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categories);
        }
        [HttpGet("{categoryId}")]
        [ProducesResponseType(200,Type= typeof(Category))]
        [ProducesResponseType(400)]
        public IActionResult GetCategory(int categoryId)
        {
            if (!_category.CategoryExist(categoryId))
                return NotFound();
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var category = _category.GetCategory(categoryId);
            return Ok(category);
        }

        [HttpGet("pokemon/{categoryId}")]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByCategory(int CategoryId)
        {
            ICollection<Pokemon> pokemons = _category.GetPokemonByCategory(CategoryId);
            if (!ModelState.IsValid)
                return BadRequest();
            
            
            return Ok(pokemons);
        }
    }
    
}
