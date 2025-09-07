using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ProfileBook.Api.Models
{
    // Use default Identity key (string) to match your AspNet* tables
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> SentMessages { get; set; } = new List<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new List<Message>();

    }
}