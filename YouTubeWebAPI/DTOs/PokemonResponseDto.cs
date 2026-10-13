namespace YouTubeWebAPI.DTOs
{
    public class PokemonResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        // Return only the related category and owner summaries.
        public List<PokemonCategoryResponseDto> Categories { get; set; } = new();
        public List<PokemonOwnerResponseDto> Owners { get; set; } = new();
    }

    public class PokemonCategoryResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PokemonOwnerResponseDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}