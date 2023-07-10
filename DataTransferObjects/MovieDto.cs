using System.ComponentModel.DataAnnotations;

namespace Movies.DataTransferObjects
{
    public class MovieDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string Description { get; set; }
    }
}
