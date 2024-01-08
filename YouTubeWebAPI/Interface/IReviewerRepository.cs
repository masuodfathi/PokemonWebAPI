using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();
        Reviewer GetReviewer(int id);
        ICollection<Review> GetReviewsByReviewer(int reviewerId);
        bool ReviewerExist(int reviewerId);
    }
}
