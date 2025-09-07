namespace ProfileBook.Api.Models;

public enum PostStatus { Pending = 0, Approved = 1, Rejected = 2 }

public class Post
{
    public int Id { get; set; }
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public string Content { get; set; } = string.Empty;
    public string? ImagePath { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public Post Post { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public ApplicationUser User { get; set; } = default!;
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class PostLike
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string UserId { get; set; } = default!;
}

public class Message
{
    public int Id { get; set; }

    // FK fields
    public string SenderId { get; set; } = default!;
    public string ReceiverId { get; set; } = default!;

    // Navigation properties
    public ApplicationUser? Sender { get; set; }
    public ApplicationUser? Receiver { get; set; }

    public string Body { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}

public class Report
{
    public int Id { get; set; }
    public string ReportedUserId { get; set; } = default!;
    public string ReportingUserId { get; set; } = default!;
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Open"; // Open, Reviewing, Actioned, Dismissed
}

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
}

public class GroupMember
{
    public int GroupId { get; set; }
    public string UserId { get; set; } = default!;
}