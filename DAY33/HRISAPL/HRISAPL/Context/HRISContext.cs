using Microsoft.EntityFrameworkCore;
using static HRISAPL.Models;

namespace HRISAPL.Context
{
    public class HRISContext:DbContext
    {
        public HRISContext(DbContextOptions<HRISContext> options):base(options)
            {

        }
        public DbSet<Department> Departments { get; set; }
       
    }
}
