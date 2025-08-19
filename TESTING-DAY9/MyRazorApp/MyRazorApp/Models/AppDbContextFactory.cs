//namespace MyRazorApp.Models
//{
//  public class AppDbContextFactory
//{
//}
//}


using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using RazorEFApp.Models;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // ⚠️ Change YOUR_SERVER_NAME if needed
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=<MyRazorApp>;Integrated Security=True;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
