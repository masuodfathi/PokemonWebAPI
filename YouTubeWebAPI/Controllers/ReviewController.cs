using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
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

        [HttpGet("pokemon/{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public ActionResult GetReviewForAPokemon(int pokemonId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var reviews = _reviewRepository.GetReviewsOfPokemon(pokemonId);
            return Ok(reviews);
        }

    }
}
