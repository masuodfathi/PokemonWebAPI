using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokeRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepository, IPokemonRepository pokemonRepository, IReviewerRepository reviewerRepository , IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _pokeRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public ActionResult GetReviews() 
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var reviews = _reviewRepository.GetReviews();
            return Ok(reviews);
        }
        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public ActionResult GetReview(int reviewId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if(!_reviewRepository.ReviewExist(reviewId))
                return NotFound();
            var review = _reviewRepository.GetReview(reviewId);
            return Ok(review);
        }

        [HttpGet("reviews/{pokemonId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public ActionResult GetReviewForAPokemon(int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var reviews = _reviewRepository.GetReviewsOfPokemon(pokemonId);
            return Ok(reviews);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview([FromBody] ReviewDto newReview)
        {
            if(newReview  == null)
                return BadRequest(ModelState);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewMap = _mapper.Map<Review>(newReview);
            if(newReview.PokemonId > 0)
            {
                var pokemon = _pokeRepository.GetPokemon(newReview.PokemonId);
                reviewMap.Pokemon = pokemon;
            }
            if(newReview.ReviewerId > 0)
            {
                var reviewer = _reviewerRepository.GetReviewer(newReview.ReviewerId);
                reviewMap.Reviewer = reviewer;
            }

            _reviewRepository.CreateReview(reviewMap);
            return Ok("Review has added successfully!");
            
        }
    }
}
