// ProfileBook.Api/Dtos/PostDtos.cs
namespace ProfileBook.Api.Dtos
{
    // For creating a post (used with multipart/form-data, so it's optional)
    public record CreatePostDto(string Content);

    // For adding a comment (JSON body)
    public record CreateCommentDto(string Text);

    // ...existing code...

    // Comment DTO (as in your sample)
    public record CommentDto(int Id, string UserId, string? UserName, string Text, DateTime CreatedAt);

    // Post DTO that exactly matches the response shape you provided
    public record PostDto(
        int Id,
        string Content,
        string? ImagePath,
        DateTime CreatedAt,
        string Status,
        UserDto User,
        int LikesCount,
        IEnumerable<CommentDto> Comments,
        bool IsMine
    );
}
