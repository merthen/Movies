namespace Movies.DataTransferObjects
{
    public class MovieDetailsDto
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public List<string> Comments { get; set; }
    }
}
