using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryConteroller : Controller
    {
        private readonly ICategoryRepository _category;
        private readonly IMapper _mapper;
        public CategoryConteroller(ICategoryRepository category, IMapper mapper)
        {
            _category = category;
            _mapper = mapper;
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
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var pokemons = _category.GetPokemonByCategory(categoryId);
            return Ok(pokemons);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if(categoryDto == null)
                return BadRequest(ModelState);

            var cat = _category.GetCategories().Where(c => c.Name.Trim().ToUpper() == categoryDto.Name.Trim().ToUpper()).FirstOrDefault();

            if(cat != null)
            {
                ModelState.AddModelError("Error", "Category already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryMap = _mapper.Map<Category>(categoryDto);
            
            if (!_category.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("Error", "Somthing went wrong! while saving.");
                return StatusCode(500,ModelState);
            }

            return Ok("Category successfully created!");

        }
    }
    
}
