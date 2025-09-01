using System.ComponentModel.DataAnnotations;

namespace SecureNotesApi.Dtos
{
    public class RegisterDto
    {
        [Required, MinLength(3)]
        public string Username { get; set; }
        [Required, MinLength(6)]
        public string Password { get; set; }
    }
}
