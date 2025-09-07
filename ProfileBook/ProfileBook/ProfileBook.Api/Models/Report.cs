using System;

namespace ProfileBook.Api.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;

        public int ReportingUserId { get; set; }
        public User ReportingUser { get; set; } = null!;

        public int ReportedUserId { get; set; }
        public User ReportedUser { get; set; } = null!;

        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
