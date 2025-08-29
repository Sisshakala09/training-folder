using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SecureMvcAuth.Models
{
    public class Test
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
