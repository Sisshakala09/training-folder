using Microsoft.EntityFrameworkCore;
//using MVCExample.Models;
using RazorApp.Models;

namespace RazorApp.Data { }
public class EcomContext : DbContext
{
    public EcomContext(DbContextOptions<EcomContext> options) : base(options) { }

    // Example DbSets
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
}

