namespace ProfileBook.Api.Dtos
{
    public record UserAdminDto(string Id, string UserName, string Email, string[] Roles, string? ProfileImage = null);

    public record UpdateUserDto(string? UserName, string? Email, string? NewPassword, string[]? Roles);

    public record CreateGroupDto(string Name, string[]? MemberIds);

    public record GroupDetailDto(int Id, string Name, IEnumerable<string> MemberIds);

    public class CreateUserWithProfileFormDto
    {
        
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; } = true;
        
        public string? Roles { get; set; }
        
        public IFormFile? ProfileImage { get; set; }
    }

    public class UpdateUserWithProfileFormDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Roles { get; set; } // comma-separated
        public IFormFile? ProfileImage { get; set; }
    }
}
