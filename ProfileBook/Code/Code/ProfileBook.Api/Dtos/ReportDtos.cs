namespace ProfileBook.Api.Dtos
{
    public record CreateReportDto(string ReportedUserId, string Reason);
    public record UpdateReportStatusDto(string Status);

    public record ReportDto(
        int Id,
        string ReportedUserId,
        string ReportedUserName,
        string ReportingUserId,
        string ReportingUserName,
        string Reason,
        string Status,
        DateTime CreatedAt
    );
}