using System.ComponentModel.DataAnnotations;

namespace SecureShop.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required, StringLength(500)]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
