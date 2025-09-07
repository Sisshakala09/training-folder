namespace ProfileBook.Api.Dtos
{
    // Shared lightweight user DTO used across posts/messages
    public record UserDto(string Id, string? UserName, string? ProfileImageUrl = null);
}
