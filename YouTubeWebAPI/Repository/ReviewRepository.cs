using YouTubeWebAPI.Data;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Repository
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;
        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateReview(Review review)
        {
            _context.Reviews.Add(review);
            return Save();
        }

        public bool DeleteReview(int reviewId)
        {
            var reviewToDelete = _context.Reviews.Find(reviewId);
            if (reviewToDelete != null)
            {
                _context.Reviews.Remove(reviewToDelete);
                return Save();
            }
            return false;
        }

        public Review GetReview(int reviewId)
        {
            return _context.Reviews.Where(r => r.Id == reviewId).FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.ToList();
        }

        public ICollection<Review> GetReviewsOfPokemon(int pokemonId)
        {
            return _context.Reviews.Where(r => r.Pokemon.Id == pokemonId).ToList();
        }

        public bool ReviewExist(int reviewId)
        {
            return _context.Reviews.Any(r => r.Id == reviewId);
        }

        public bool Save()
        {
            try
            {
                var save = _context.SaveChanges();
                return save > 0 ? true : false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateReview(Review review)
        {
            _context.Reviews.Update(review);
            return Save();
        }
    }
}
