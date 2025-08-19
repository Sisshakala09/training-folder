using System.ComponentModel.DataAnnotations.Schema;
using RazorEFApp.Models;

namespace MyRazorApp.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // 👇 Explicitly tell EF Core how to store decimals in SQL Server
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}
