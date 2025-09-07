namespace ProfileBook.Api.Dtos
{
    // Used by PUT /api/users/profile
    public class UpdateUserProfileDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }

        // To change password: supply current and new password.
        public string? CurrentPassword { get; set; }
        public string? NewPassword { get; set; }

        public string? Bio { get; set; } // optional; wire to ApplicationUser.Bio if you add it
    }
}
