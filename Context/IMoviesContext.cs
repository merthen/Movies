using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies.Context
{
    public interface IMoviesContext
    {
        DbSet<Movie> Movies { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Comment> Comments { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Rating> Ratings { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
