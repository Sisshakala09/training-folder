using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Models;
using ProfileBook.Api.Models.Dto;
using System.Security.Claims;

namespace ProfileBook.Api.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class PostsController : ControllerBase {
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public PostsController(AppDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    [HttpGet("approved")]
    public async Task<IActionResult> GetApproved() {
      var posts = await _db.Posts.Include(p => p.User).Where(p => p.Status == PostStatus.Approved).OrderByDescending(p => p.CreatedAt).ToListAsync();
      return Ok(posts);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromForm] CreatePostDto dto) {
      var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      if (!int.TryParse(claim, out var userId)) return Unauthorized();

      string? imageUrl = null;
      if (dto.Image != null && dto.Image.Length > 0) {
        var uploads = Path.Combine(_env.WebRootPath ?? "wwwroot", "uploads");
        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
        var filename = $"{Guid.NewGuid()}{Path.GetExtension(dto.Image.FileName)}";
        var filePath = Path.Combine(uploads, filename);
        using var stream = System.IO.File.Create(filePath);
        await dto.Image.CopyToAsync(stream);
        imageUrl = $"/uploads/{filename}";
      }

      var post = new Post { UserId = userId, Content = dto.Content, ImageUrl = imageUrl, Status = PostStatus.Pending };
      _db.Posts.Add(post);
      await _db.SaveChangesAsync();
      return Ok(post);
    }

    [HttpPut("{id}/approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Approve(int id) {
      var post = await _db.Posts.FindAsync(id);
      if (post == null) return NotFound();
      post.Status = PostStatus.Approved;
      await _db.SaveChangesAsync();
      return Ok(post);
    }
  }
}
