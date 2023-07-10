namespace Movies.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Movies")]
    public class Movie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ReleaseDate { get; set; }

        public string Description { get; set; }

        public double Rating { get; set; }

        public List<Comment> Comments { get; set; }
        public List<Rating> Ratings { get; set; }
        public double AverageRating
        {
            get
            {
                if (Ratings.Count > 0)
                {
                    return Ratings.Average(r => r.Score);
                }
                return 0;
            }
        }
    }
}
