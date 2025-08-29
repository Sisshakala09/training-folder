using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecureMvcAuth.Models;

namespace SecureMvcAuth.Data
{
    public class ApplicationDbContext : IdentityDbContext<Test>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }
    }
}





