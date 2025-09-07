namespace ProfileBook.Api.Services
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string userName, string email, IEnumerable<string> roles);
    }
}