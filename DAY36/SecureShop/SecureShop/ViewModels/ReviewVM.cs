using System.ComponentModel.DataAnnotations;

namespace SecureShop.ViewModels
{
    public class ReviewVM
    {
        public int ProductId { get; set; }

        [Required, StringLength(500)]
        public string Content { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }
    }
}
