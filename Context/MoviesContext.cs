using Microsoft.EntityFrameworkCore;
using Movies.Models;

namespace Movies.Context
{
    public class MoviesContext : DbContext, IMoviesContext
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public MoviesContext(DbContextOptions<MoviesContext> options) : base(options)
        {
        }

        public MoviesContext() : base() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships between entities
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Movie)
                .WithMany(m => m.Comments)
                .HasForeignKey(c => c.MovieId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasMany(u => u.InterestedCategories)
                .WithMany()
                .UsingEntity(x => x.ToTable("UserCategory"));

            modelBuilder.Entity<Movie>()
                .HasOne(m => m.Category)
                .WithMany()
                .HasForeignKey(m => m.CategoryId);

            modelBuilder.Entity<Movie>()
                .HasMany(m => m.Ratings)
                .WithOne(r => r.Movie)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
