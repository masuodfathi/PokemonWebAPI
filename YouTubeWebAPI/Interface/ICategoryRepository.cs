using YouTubeWebAPI.Models;

namespace YouTubeWebAPI.Interface
{
    public interface ICategoryRepository
    {
        ICollection<Category> GetCategories();
        Category GetCategory(int id);
        ICollection<Pokemon> GetPokemonByCategory(int categoryId);
        bool CategoryExist(int Id);
        bool CreateCategory(Category category);
        bool Save();
    }
}
