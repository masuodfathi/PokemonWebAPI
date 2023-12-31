namespace YouTubeWebAPI.Models
{
    public class PokemonCategory
    {
        public int PokemonId { get; set; }
        public int CategoryID { get; set; }
        public Pokemon pokemon { get; set; }
        public Category category { get; set; }
    }
}
