using Microsoft.EntityFrameworkCore;
using YouTubeWebAPI.Data;
using YouTubeWebAPI.Interface;
using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _context;
        public ReviewerRepository(DataContext context)
        {
            _context = context;
        }
        public Reviewer GetReviewer(int id)
        {
            return _context.Reviewers.Where(r => r.Id == id).Include(re=>re.Reviews).FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _context.Reviewers.ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _context.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExist(int reviewerId)
        {
            return _context.Reviewers.Any(r => r.Id == reviewerId);
        }
    }
}
