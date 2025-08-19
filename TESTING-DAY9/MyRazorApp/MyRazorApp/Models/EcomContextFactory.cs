using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class EcomContextFactory : IDesignTimeDbContextFactory<EcomContext>
{
    public EcomContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EcomContext>();

        // ⚠️ change YOUR_SERVER_NAME to your actual SQL Server name
        optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=EcomDb;Integrated Security=True;TrustServerCertificate=True");

        return new EcomContext(optionsBuilder.Options);
    }
}
