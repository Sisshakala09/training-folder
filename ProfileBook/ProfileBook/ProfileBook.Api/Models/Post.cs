using System;

namespace ProfileBook.Api.Models {
  public enum PostStatus { Pending, Approved, Rejected }

  public class Post {
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public string Content { get; set; } = "";
    public string? ImageUrl { get; set; }
    public PostStatus Status { get; set; } = PostStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}
