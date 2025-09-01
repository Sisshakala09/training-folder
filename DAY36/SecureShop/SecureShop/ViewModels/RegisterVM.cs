using System.ComponentModel.DataAnnotations;

namespace SecureShop.ViewModels
{
    public class RegisterVM
    {
        [Required, StringLength(50)]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$",
            ErrorMessage = "Password must be 8+ chars, one uppercase, one number and one special char.")]
        public string Password { get; set; }

        [Required, DataType(DataType.Password), Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
