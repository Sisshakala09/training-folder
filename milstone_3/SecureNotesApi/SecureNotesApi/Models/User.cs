using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SecureNotesApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public ICollection<Note> Notes { get; set; }
    }
}
