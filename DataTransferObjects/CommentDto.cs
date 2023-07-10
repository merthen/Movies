using System.ComponentModel.DataAnnotations;

namespace Movies.DataTransferObjects
{
    public class CommentDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
