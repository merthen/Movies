using Microsoft.AspNetCore.Mvc;
using Moq;
using Movies.Controllers;
using Movies.Context;
using Movies.DataTransferObjects;
using Movies.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Movies.Responses;

namespace Movies.Tests
{
    [TestFixture]
    public class MoviesControllerTests
    {
        private MoviesController _controller;
        private Mock<IMoviesContext> _mockContext;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<IMoviesContext>();
            _controller = new MoviesController(_mockContext.Object);
        }

        [Test]
        public async Task AddMovie_ValidMovieDto_ReturnsOkWithMovieId()
        {
            // Arrange
            var movieDto = new MovieDto
            {
                Title = "Test Movie",
                CategoryId = 1,
                ReleaseDate = new DateTime(2022, 1, 1),
                Description = "Test Description"
            };

            _mockContext.Setup(c => c.Categories.FindAsync(movieDto.CategoryId))
                .ReturnsAsync(new Category { Id = movieDto.CategoryId });

            var movieId = 1;
            _mockContext.Setup(c => c.Movies.Add(It.IsAny<Movie>()))
                .Callback<Movie>(movie =>
                {
                    movie.Id = movieId;
                    movie.Ratings = new List<Rating>();
                    movie.Comments = new List<Comment>();
                });
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.AddMovie(movieDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as AddMovieResponse;
            Assert.AreEqual(movieId, response.MovieId);
        }

        [Test]
        public async Task RateMovie_ValidMovieIdAndRatingDto_AddsRatingToMovie()
        {
            // Arrange
            var movieId = 1;
            var ratingDto = new RatingDto
            {
                UserId = 1,
                Score = 5
            };
            var movie = new Movie
            {
                Id = movieId,
                Title = "Test Movie",
                CategoryId = 1,
                ReleaseDate = new DateTime(2022, 1, 1),
                Description = "Test Description",
                Ratings = new List<Rating>(),
                Comments = new List<Comment>()
            };
            _mockContext.Setup(c => c.Movies.FindAsync(movieId))
                .ReturnsAsync(movie);
            _mockContext.Setup(c => c.SaveChangesAsync(CancellationToken.None))
                .ReturnsAsync(1);

            // Act
            var result = await _controller.RateMovie(movieId, ratingDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Additional assertions based on the expected behavior
            Assert.AreEqual("Rating added successfully.", result.Value);
            Assert.AreEqual(1, movie.Ratings.Count);
            Assert.AreEqual(ratingDto.UserId, movie.Ratings.First().UserId);
            Assert.AreEqual(ratingDto.Score, movie.Rating);
        }

        [Test]
        public async Task AddComment_ValidMovieIdAndCommentDto_AddsCommentToMovie()
        {
            // Arrange
            var movieId = 1;
            var commentDto = new CommentDto
            {
                UserId = 1,
                Text = "Test Comment"
            };
            var movie = new Movie
            {
                Id = movieId,
                Title = "Test Movie",
                CategoryId = 1,
                ReleaseDate = new DateTime(2022, 1, 1),
                Description = "Test Description",
                Ratings = new List<Rating>(),
                Comments = new List<Comment>()
            };
            _mockContext.Setup(c => c.Movies.FindAsync(movieId))
                .ReturnsAsync(movie);

            var user = new User
            {
                Id = commentDto.UserId,
                Name = "Test User",
                Comments = new List<Comment>(),
                InterestedCategories = new List<Category>()
            };
            _mockContext.Setup(c => c.Users.FindAsync(commentDto.UserId))
                .ReturnsAsync(user);

            // Create a mock DbSet<Comment> using MockDbSetHelper
            var comments = new List<Comment>();
            var mockCommentsDbSet = MockDbSetHelper.CreateMockDbSet(comments);
            _mockContext.Setup(c => c.Comments).Returns(mockCommentsDbSet.Object);

            // Act
            var result = await _controller.AddComment(movieId, commentDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual("Comment added successfully.", result.Value);
        }

        [Test]
        public async Task SearchMovies_ValidTitle_ReturnsMatchingMovies()
        {
            // Arrange
            var title = "Test";
            var movies = new List<Movie>
    {
        new Movie
        {
            Id = 1,
            Title = "Test Movie 1",
            CategoryId = 1,
            ReleaseDate = new DateTime(2022, 1, 1),
            Description = "Test Description 1"
        },
        new Movie
        {
            Id = 2,
            Title = "Movie 2",
            CategoryId = 2,
            ReleaseDate = new DateTime(2022, 1, 1),
            Description = "Test Description 2"
        },
        new Movie
        {
            Id = 3,
            Title = "Test Movie 3",
            CategoryId = 3,
            ReleaseDate = new DateTime(2022, 1, 1),
            Description = "Test Description 3"
        }
    };

            var mockDbSet = MockDbSetHelper.CreateMockDbSet(movies);
            _mockContext.Setup(c => c.Movies).Returns(mockDbSet.Object);

            // Act
            var result = await _controller.SearchMovies(title) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(200, result.StatusCode);

            // Additional assertions based on the expected behavior
            var movieList = result.Value as List<Movie>;
            Assert.NotNull(movieList);
            Assert.AreEqual(2, movieList.Count);
            Assert.IsTrue(movieList.All(m => m.Title.Contains(title)));
        }
    }
}
