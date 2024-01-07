using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int reviewId);
        ICollection<Review> GetReviewsOfPokemon(int pokemonId);
        bool ReviewExist(int reviewId);
    }
}
