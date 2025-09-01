using System.Collections.Generic;
using System.Reflection.Emit;
using AdvancedLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace AdvancedLibrary.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

        public DbSet<Book> Books => Set<Book>();
        public DbSet<Author> Authors => Set<Author>();
        public DbSet<Genre> Genres => Set<Genre>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Book–Genre Many-to-Many via shadow join table
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BookGenres"));

            // Seed minimal data
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "Unknown Author" },
                new Author { Id = 2, Name = "Jane Austen" }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fiction" },
                new Genre { Id = 2, Name = "Romance" },
                new Genre { Id = 3, Name = "Classic" }
            );

            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "Pride and Prejudice", PublishedYear = 1813, AuthorId = 2 }
            );
        }
    }
}
