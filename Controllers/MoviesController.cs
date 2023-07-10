namespace Movies.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Movies.Context;
    using Movies.DataTransferObjects;
    using Movies.Models;
    using Movies.Responses;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    [ApiController]
    [Route("api/movies")]
    public class MoviesController : ControllerBase
    {
        private readonly IMoviesContext _context;
        private int myPageSize = 10;
        public int PageSize { get { return myPageSize; } }

        public MoviesController(IMoviesContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddMovie(MovieDto movieDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _context.Categories.FindAsync(movieDto.CategoryId);
            if (category == null)
            {
                return BadRequest("Invalid category ID.");
            }

            var movie = new Movie
            {
                Title = movieDto.Title,
                CategoryId = movieDto.CategoryId,
                ReleaseDate = movieDto.ReleaseDate,
                Description = movieDto.Description,
                Ratings = new List<Rating>(),
                Comments = new List<Comment>()
            };

            _context.Movies.Add(movie);
            await _context.SaveChangesAsync(CancellationToken.None);

            var response = new AddMovieResponse(movie.Id);

            return Ok(response);
        }

        [HttpPost("{movieId}/rating")]
        public async Task<IActionResult> RateMovie(int movieId, RatingDto ratingDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }

            // Perform any necessary validation on ratingDto.Score

            // Create a new rating
            var rating = new Rating
            {
                MovieId = movieId,
                UserId = ratingDto.UserId,
                Score = ratingDto.Score
            };

            // Add the rating to the movie's collection
            movie.Ratings.Add(rating);

            // Recalculate the average rating
            movie.Rating = movie.AverageRating;

            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok("Rating added successfully.");
        }

        [HttpPost("{movieId}/comments")]
        public async Task<IActionResult> AddComment(int movieId, CommentDto commentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var movie = await _context.Movies.FindAsync(movieId);
            if (movie == null)
            {
                return NotFound();
            }
            var user = await _context.Users.FindAsync(commentDto.UserId);
            if (user == null)
            {
                return NotFound();
            }

            // Create a new comment
            var comment = new Comment
            {
                MovieId = movieId,
                Movie = movie,
                UserId = commentDto.UserId,
                User = user,
                Text = commentDto.Text
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync(CancellationToken.None);

            return Ok("Comment added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> SearchMovies(string title, int page = 1)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title parameter is required.");
            }

            var movies = await _context.Movies
                .Where(m => m.Title.Contains(title))
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            return Ok(movies);
        }

        [HttpGet("{movieId}")]
        public async Task<IActionResult> GetMovie(int movieId)
        {
            var movie = await _context.Movies
                .Include(m => m.Category)
                .Include(m => m.Comments)
                .FirstOrDefaultAsync(m => m.Id == movieId);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDetails = new MovieDetailsDto
            {
                Title = movie.Title,
                Category = movie.Category.Name,
                ReleaseDate = movie.ReleaseDate,
                Description = movie.Description,
                AverageRating = movie.Rating,
                Comments = movie.Comments.Select(c => c.Text).ToList()
            };

            return Ok(movieDetails);
        }

        [HttpGet("users/{userId}/recommendations")]
        public IActionResult GetMovieRecommendations(int userId)
        {
            var user = _context.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            var recommendations = _context.Movies.Where(m => user.InterestedCategories.Contains(m.Category)).ToList();

            return Ok(recommendations);
        }
    }
}
