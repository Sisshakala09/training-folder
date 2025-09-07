using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Dtos;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PostsController(ApplicationDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string? q)
        {
            // Determine caller info
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                        ?? User.FindFirst("uid")?.Value;
            var isAdmin = User.IsInRole("Admin");

            // Base query: include needed navigation so we can count likes and fetch comment user names
            var query = _db.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .AsQueryable();

            // Apply filter rules:
            if (!isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(currentUserId))
                {
                    query = query.Where(p => p.Status == PostStatus.Approved || p.UserId == currentUserId);
                }
                else
                {
                    query = query.Where(p => p.Status == PostStatus.Approved);
                }
            }

            // Optional free-text search on post content
            if (!string.IsNullOrWhiteSpace(q))
            {
                // simple contains search; DB collation usually handles case-insensitivity
                query = query.Where(p => p.Content.Contains(q));
            }

            // EF-friendly projection to pull required fields
            var itemsRaw = await query
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.ImagePath,
                    p.CreatedAt,
                    Status = p.Status.ToString(),
                    // user info
                    UserId = p.UserId,
                    UserName = p.User != null ? p.User.UserName : null,
                    ProfileImage = p.User != null ? p.User.ProfileImage : null, // ApplicationUser.ProfileImage
                    // likes count
                    LikesCount = p.Likes.Count,
                    // comments (ordered)
                    Comments = p.Comments
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new
                        {
                            c.Id,
                            c.UserId,
                            UserName = c.User != null ? c.User.UserName : null,
                            c.Text,
                            c.CreatedAt
                        })
                        .ToList()
                })
                .ToListAsync();

            // Map to DTOs in memory (so we can use Request and helpers)
            var items = itemsRaw.Select(p => new PostDto(
                Id: p.Id,
                Content: p.Content,
                ImagePath: GetAbsoluteUrl(p.ImagePath),
                CreatedAt: p.CreatedAt,
                Status: p.Status,
                User: new UserDto(
                    p.UserId ?? string.Empty,
                    p.UserName ?? string.Empty,
                    GetAbsoluteUrl(p.ProfileImage)
                ),
                LikesCount: p.LikesCount,
                Comments: p.Comments.Select(c => new CommentDto(c.Id, c.UserId, c.UserName, c.Text, c.CreatedAt)).ToList(),
                IsMine: !string.IsNullOrWhiteSpace(currentUserId) && p.UserId == currentUserId
            )).ToList();

            return Ok(items);
        }
        

        [AllowAnonymous]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Determine current user info
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                                        ?? User.FindFirst("uid")?.Value;
            var isAdmin = User.IsInRole("Admin");

            // Query including necessary relations
            var query = _db.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .Where(p => p.Id == id);

            // Apply filtering (if not admin)
            if (!isAdmin)
            {
                if (!string.IsNullOrWhiteSpace(currentUserId))
                {
                    query = query.Where(p => p.Status == PostStatus.Approved || p.UserId == currentUserId);
                }
                else
                {
                    query = query.Where(p => p.Status == PostStatus.Approved);
                }
            }

            var post = await query.FirstOrDefaultAsync();
            if (post == null) return NotFound();

            // Map to DTO
            var dto = new PostDto(
                Id: post.Id,
                Content: post.Content,
                ImagePath: GetAbsoluteUrl(post.ImagePath),
                CreatedAt: post.CreatedAt,
                Status: post.Status.ToString(),
                User: new UserDto(
                    post.User?.Id ?? string.Empty,
                    post.User?.UserName ?? string.Empty,
                    GetAbsoluteUrl(post.User?.ProfileImage)
                ),
                LikesCount: post.Likes.Count,
                Comments: post.Comments
                    .OrderBy(c => c.CreatedAt)
                    .Select(c => new CommentDto(
                        c.Id,
                        c.UserId,
                        c.User?.UserName ?? string.Empty,
                        c.Text,
                        c.CreatedAt
                    )).ToList(),
                IsMine: !string.IsNullOrWhiteSpace(currentUserId) && post.UserId == currentUserId
            );

            return Ok(dto);
}



        // ===== CREATE post (multipart: content, image?) =====
        [Authorize]
        [HttpPost]
        [RequestSizeLimit(10_000_000)] // ~10MB
        public async Task<IActionResult> Create([FromForm] string content, IFormFile? image)
        {
            if (string.IsNullOrWhiteSpace(content) && image == null)
                return BadRequest(new { error = "Provide content or an image." });

            // Admins are not allowed to create posts
            if (User.IsInRole("Admin"))
                return Forbid();

            string? relPath = null;

            if (image is { Length: > 0 })
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var folder = Path.Combine(webRoot, "uploads", "posts");
                Directory.CreateDirectory(folder);

                var file = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                var fullPath = Path.Combine(folder, file);
                await using var fs = System.IO.File.Create(fullPath);
                await image.CopyToAsync(fs);
                relPath = $"/uploads/posts/{file}";
            }

            var uid = GetUserId();
            var post = new Post
            {
                UserId = uid,
                Content = content,
                ImagePath = relPath
                // Status defaults to Pending; CreatedAt set by model
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();

            // Fetch the user to populate nested user object (and profile image)
            var user = await _db.Users.FindAsync(uid);
            var profileImage = user?.ProfileImage;

            // Build empty comment list for the newly created post
            var comments = new List<CommentDto>();

            // Build the nested UserDto (ProfileImageUrl may be null)
            var userDto = new UserDto(
                user?.Id ?? uid,
                user?.UserName ?? string.Empty,
                GetAbsoluteUrl(profileImage)
            );

            // Build the PostDto that matches the response shape:
            var result = new PostDto(
                Id: post.Id,
                Content: post.Content,
                ImagePath: post.ImagePath,
                CreatedAt: post.CreatedAt,
                Status: post.Status.ToString(),
                User: userDto,
                LikesCount: 0,
                Comments: comments,
                IsMine: true
            );

            return Ok(result);
        }


        // ===== ADMIN: approve =====
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();

            post.Status = PostStatus.Approved;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ===== ADMIN: reject =====
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var post = await _db.Posts.FindAsync(id);
            if (post == null) return NotFound();

            post.Status = PostStatus.Rejected;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ===== LIKE =====
        [Authorize]
        [HttpPost("{id:int}/like")]
        public async Task<IActionResult> Like(int id)
        {
            var uid = GetUserId();

            var postExists = await _db.Posts.AnyAsync(p => p.Id == id && p.Status == PostStatus.Approved);
            if (!postExists) return NotFound();

            var exists = await _db.PostLikes.AnyAsync(x => x.PostId == id && x.UserId == uid);
            if (exists) return BadRequest(new { error = "Already liked." });

            _db.PostLikes.Add(new PostLike { PostId = id, UserId = uid });
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ===== UNLIKE =====
        [Authorize]
        [HttpDelete("{id:int}/like")]
        public async Task<IActionResult> Unlike(int id)
        {
            var uid = GetUserId();
            var like = await _db.PostLikes.FirstOrDefaultAsync(x => x.PostId == id && x.UserId == uid);
            if (like == null) return NotFound();

            _db.PostLikes.Remove(like);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ===== GET COMMENTS =====
        [AllowAnonymous]
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetComments(int id)
        {
            var comments = await _db.Comments
                .AsNoTracking()
                .Where(c => c.PostId == id)
                .OrderBy(c => c.CreatedAt)
                .Select(c => new CommentDto(
                    c.Id,
                    c.UserId,
                    c.User != null ? c.User.UserName ?? string.Empty : string.Empty,
                    c.Text,
                    c.CreatedAt))
                .ToListAsync();

            return Ok(comments);
        }

        // ===== ADD COMMENT (expects JSON: { "text": "..." }) =====
        [Authorize]
        [HttpPost("{id:int}/comments")]
        public async Task<IActionResult> AddComment(int id, [FromBody] CreateCommentDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Text))
                return BadRequest(new { error = "Text is required." });

            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == id && p.Status == PostStatus.Approved);
            if (post == null) return NotFound();

            var uid = GetUserId();
            var comment = new Comment { PostId = id, UserId = uid, Text = dto.Text };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            var userName = (await _db.Users.FindAsync(uid))?.UserName ?? string.Empty;
            var dtoOut = new CommentDto(comment.Id, comment.UserId, userName, comment.Text, comment.CreatedAt);

            return CreatedAtAction(nameof(GetComments), new { id }, dtoOut);
        }

        // ===== helper =====
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("uid")?.Value
                ?? User.Identity?.Name
                ?? throw new InvalidOperationException("User id not found in token.");
        }

        private string? GetAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return null;

            // If already an absolute URL, return as-is
            if (relativePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                relativePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return relativePath;
            }

            // If DB stores only a filename (like "abc.jpg") and your images live under /uploads/users/,
            // uncomment and adjust the next line:
            // relativePath = relativePath.Contains("/") ? relativePath : "/uploads/users/" + relativePath;

            var path = relativePath.StartsWith('/') ? relativePath : "/" + relativePath;
            return $"{Request.Scheme}://{Request.Host}{path}";
        }




    }
}