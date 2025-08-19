//namespace MyRazorApp.Models
//{
//  public class AppDbContext
//{
//}
//}


using Microsoft.EntityFrameworkCore;
using MyRazorApp.Models;

namespace RazorEFApp.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // 👇 Add this method
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // fallback for design-time
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MyRazorApp;Integrated Security=True;TrustServerCertificate=True");
            }
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
