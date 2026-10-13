using System.ComponentModel.DataAnnotations;

namespace YouTubeWebAPI.DTOs
{
    public class UpdatePokemonDto
    {
        // Identify the Pokemon to update.
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        // Provide the complete desired list of owners.
        [Required]
        [MinLength(1)]
        public List<int> OwnerIds { get; set; } = new();

        // Provide the complete desired list of categories.
        [Required]
        [MinLength(1)]
        public List<int> CategoryIds { get; set; } = new();
    }
}