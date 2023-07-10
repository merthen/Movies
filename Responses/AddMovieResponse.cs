namespace Movies.Responses
{
    public class AddMovieResponse
    {
        public int MovieId { get; }

        public AddMovieResponse(int id)
        {
            MovieId = id;
        }
    }
}
