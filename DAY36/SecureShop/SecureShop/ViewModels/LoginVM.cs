using System.ComponentModel.DataAnnotations;

namespace SecureShop.ViewModels
{
    public class LoginVM
    {
        [Required] public string Username { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
    }
}
