using System.ComponentModel.DataAnnotations;

namespace Movies.DataTransferObjects
{
    public class RatingDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        [Range(1, 100)]
        public int Score { get; set; }
    }
}
