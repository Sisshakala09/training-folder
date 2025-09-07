namespace ProfileBook.Api.Dtos
{
    public record RegisterDto(string UserName, string Email, string Password);
    public record LoginDto(string UserNameOrEmail, string Password);
}