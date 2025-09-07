using System;
using System.Collections.Generic;

namespace ProfileBook.Api.Dtos
{
    public record PostSummaryDto(
        int Id,
        string Content,
        string? ImagePath,
        DateTime CreatedAt,
        string Status,
        int LikesCount,
        IEnumerable<CommentDto> Comments
    );

    public record UserSearchResultDto(string Id, string UserName, string? Email, string? ProfileImage, IEnumerable<PostSummaryDto> Posts);
}
