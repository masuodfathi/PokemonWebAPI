using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using YouTubeWebAPI.DTOs;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;
using YouTubeWebAPI.Repository;

namespace YouTubeWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewers()
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviewers = _reviewerRepository.GetReviewers();
            return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewerById(int reviewerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(!_reviewerRepository.ReviewerExist(reviewerId))
                return NotFound();

            var reviewer = _reviewerRepository.GetReviewer(reviewerId);
            return Ok(reviewer);
        }

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewerId(int reviewerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExist(reviewerId))
                return NotFound();

            var reviews = _reviewerRepository.GetReviewsByReviewer(reviewerId);
            return Ok(reviews);
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto newReviewer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if(newReviewer == null)
                return BadRequest(ModelState);

            var reviewerMap = _mapper.Map<Reviewer>(newReviewer);
            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("Save error", "Somthing went wrong while saving...!");
                return StatusCode(500,ModelState);
            }
            return Ok("New reviewer added successfully!");
        }
        [HttpPut]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult UpdateReviewer(ReviewerDto updatedreviewer)
        {
            if (updatedreviewer == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewerRepository.ReviewerExist(updatedreviewer.Id))
            {
                ModelState.AddModelError("Error", "Not Found");
                return StatusCode(404, ModelState);
            }

            var updatedReviewerMap = _mapper.Map<Reviewer>(updatedreviewer);
            if (!_reviewerRepository.UpdateReviewer(updatedReviewerMap))
            {
                ModelState.AddModelError("Server Error", "Something went wrong while saving...!");
                return StatusCode(500, ModelState);
            }

            return Ok("Reviewer updated successfully");
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExist(reviewerId))
            {
                ModelState.AddModelError("Error", "Reviewer does not exist!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.DeleteReviewer(reviewerId))
            {
                ModelState.AddModelError("Server Error", "Somthing went wrong while deleting reviewer...!");
                return StatusCode(500, ModelState);
            }
            return Ok("Reviewer deleted!");
        }
    }
}
