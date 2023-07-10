namespace Movies
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Movies.Context;
    using Movies.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new MoviesContext(
                serviceProvider.GetRequiredService<DbContextOptions<MoviesContext>>());

            if (context.Movies.Any() || context.Users.Any() || context.Categories.Any() || context.Comments.Any() || context.Ratings.Any())
            {
                // Database has already been seeded
                return;
            }

            var categories = new List<Category>
        {
            new Category { Name = "Action" },
            new Category { Name = "Comedy" },
            new Category { Name = "Drama" },
            new Category { Name = "Horror" },
            new Category { Name = "Sci-Fi" },
            new Category { Name = "Crime" }
        };

            context.Categories.AddRange(categories);
            context.SaveChanges();

            var movies = new List<Movie>
        {
            new Movie
            {
                Title = "The Shawshank Redemption",
                CategoryId = categories.First(c => c.Name == "Drama").Id,
                ReleaseDate = new DateTime(1994, 10, 14),
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                Ratings = new List<Rating>()
            },
            new Movie
            {
                Title = "The Dark Knight",
                CategoryId = categories.First(c => c.Name == "Action").Id,
                ReleaseDate = new DateTime(2008, 7, 18),
                Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                Ratings = new List<Rating>()
            },
            new Movie
            {
                Title = "Pulp Fiction",
                CategoryId = categories.First(c => c.Name == "Crime").Id,
                ReleaseDate = new DateTime(1994, 10, 14),
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                Ratings = new List<Rating>()
            }
        };

            context.Movies.AddRange(movies);
            context.SaveChanges();

            var users = new List<User>
        {
            new User
            {
                Name = "John Doe",
                InterestedCategories = categories.Where(c => c.Name == "Action" || c.Name == "Sci-Fi").ToList(),
                Comments = new List<Comment>()
            },
            new User
            {
                Name = "Jane Smith",
                InterestedCategories = categories.Where(c => c.Name == "Comedy" || c.Name == "Drama").ToList(),
                Comments = new List<Comment>()
            }
        };

            context.Users.AddRange(users);
            context.SaveChanges();

            var comments = new List<Comment>
        {
            new Comment
            {
                UserId = users.First(u => u.Name == "John Doe").Id,
                MovieId = movies.First(m => m.Title == "The Shawshank Redemption").Id,
                Text = "Great movie!"
            },
            new Comment
            {
                UserId = users.First(u => u.Name == "Jane Smith").Id,
                MovieId = movies.First(m => m.Title == "The Shawshank Redemption").Id,
                Text = "One of my favorites!"
            }
        };

            context.Comments.AddRange(comments);
            context.SaveChanges();

            var ratings = new List<Rating>
        {
            new Rating
            {
                UserId = users.First(u => u.Name == "John Doe").Id,
                MovieId = movies.First(m => m.Title == "The Shawshank Redemption").Id,
                Score = 95
            },
            new Rating
            {
                UserId = users.First(u => u.Name == "Jane Smith").Id,
                MovieId = movies.First(m => m.Title == "The Shawshank Redemption").Id,
                Score = 90
            }
        };

            context.Ratings.AddRange(ratings);
            context.SaveChanges();

            foreach (var rating in ratings)
            {
                var movie = context.Movies.FirstOrDefaultAsync(m => m.Id == rating.MovieId).Result;
                movie.Ratings.Add(rating);
            }

            context.SaveChanges();
        }
    }
}
