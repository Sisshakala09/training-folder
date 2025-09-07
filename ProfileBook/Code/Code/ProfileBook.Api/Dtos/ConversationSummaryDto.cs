namespace ProfileBook.Api.Dtos
{
    public record ConversationSummaryDto(
        string UserId,
        string UserName,
        string? Email,
        MessageDto? LastMessageFromUserToMe,
        MessageDto? LastMessageOverall,
        int UnreadCount
    );
}
