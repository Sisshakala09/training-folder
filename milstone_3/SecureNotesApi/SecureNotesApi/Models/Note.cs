using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecureNotesApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
