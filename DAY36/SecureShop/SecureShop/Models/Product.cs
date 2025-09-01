using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureShop.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
