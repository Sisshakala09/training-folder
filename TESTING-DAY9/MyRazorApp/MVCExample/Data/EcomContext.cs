using Microsoft.EntityFrameworkCore;
using MVCExample.Models;

namespace MVCExample.Data { }
public class EcomContext : DbContext
{
    public EcomContext(DbContextOptions<EcomContext> options) : base(options) { }

    // Example DbSets
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
}

