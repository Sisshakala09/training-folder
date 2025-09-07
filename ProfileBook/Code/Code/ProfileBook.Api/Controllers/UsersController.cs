using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Dtos;
using ProfileBook.Api.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ProfileBook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public UsersController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _db = db;
            _env = env;
        }

        // GET /api/users/{id}
        // Returns UserSearchResultDto including posts with likes count and comments
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Id is required.");

            // Find user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Caller info & admin status
            var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("uid")?.Value;
            bool callerIsAdmin = false;
            if (!string.IsNullOrEmpty(callerId))
            {
                var caller = await _userManager.FindByIdAsync(callerId);
                if (caller != null)
                    callerIsAdmin = await _userManager.IsInRoleAsync(caller, "Admin");
            }

            // Query posts for this user, include Likes and Comments and comment.User for username
            var postsQuery = _db.Posts
                .AsNoTracking()
                .Where(p => p.UserId == id)
                .Include(p => p.Likes)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                .OrderByDescending(p => p.CreatedAt)
                .AsQueryable();

            // If caller is not admin and not the owner, only show approved posts
            if (!callerIsAdmin && !string.Equals(callerId, id, StringComparison.OrdinalIgnoreCase))
            {
                postsQuery = postsQuery.Where(p => p.Status == PostStatus.Approved);
            }

            // Project to PostSummaryDto
            var posts = await postsQuery
                .Select(p => new PostSummaryDto(
                    p.Id,
                    p.Content,
                    p.ImagePath,
                    p.CreatedAt,
                    p.Status.ToString(),
                    p.Likes.Count,
                    p.Comments
                        .OrderBy(c => c.CreatedAt)
                        .Select(c => new CommentDto(
                            c.Id,
                            c.UserId,
                            c.User != null ? c.User.UserName : string.Empty,
                            c.Text,
                            c.CreatedAt
                        ))
                        .ToList()
                ))
                .ToListAsync();

            var result = new UserSearchResultDto(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email,
                // If you want absolute URL: use GetAbsoluteUrl(user.ProfileImage)
                user.ProfileImage,
                posts
            );

            return Ok(result);
        }



        // GET /api/users/search?query=...
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var q = $"%{query.Trim()}%";

            // connection string from configuration
            var connStr = _db.Database.GetDbConnection().ConnectionString;

            var matchedUsers = new List<(string Id, string UserName, string? Email, string? ProfileImage)>();

            // 1) Query users with simple parameterized SQL (no EF translation)
            using (var conn = new SqlConnection(connStr))
            {
                await conn.OpenAsync();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT TOP (50) [Id], [UserName], [Email], [ProfileImage]
                        FROM [AspNetUsers]
                        WHERE (UserName LIKE @q) OR (Email LIKE @q);
                    ";
                    cmd.Parameters.Add(new SqlParameter("@q", SqlDbType.NVarChar, 4000) { Value = q });

                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        matchedUsers.Add((
                            Id: reader.GetString(0),
                            UserName: reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Email: reader.IsDBNull(2) ? null : reader.GetString(2),
                            ProfileImage: reader.IsDBNull(3) ? null : reader.GetString(3)
                        ));
                    }
                }

                if (!matchedUsers.Any())
                    return Ok(Array.Empty<object>());

                // Determine caller admin status
                bool callerIsAdmin = false;
                var callerId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("uid")?.Value;
                if (!string.IsNullOrEmpty(callerId))
                {
                    var caller = await _userManager.FindByIdAsync(callerId);
                    if (caller != null)
                        callerIsAdmin = await _userManager.IsInRoleAsync(caller, "Admin");
                }

                // 2) For each user fetch posts (small N, acceptable). Use parameterized SQL.
                var results = new List<object>();
                foreach (var u in matchedUsers)
                {
                    using var postCmd = conn.CreateCommand();
                    if (callerIsAdmin)
                    {
                        postCmd.CommandText = @"
                            SELECT Id, Content, ImagePath, CreatedAt, Status
                            FROM Posts
                            WHERE UserId = @uid
                            ORDER BY CreatedAt DESC;
                        ";
                    }
                    else
                    {
                        postCmd.CommandText = @"
                            SELECT Id, Content, ImagePath, CreatedAt, Status
                            FROM Posts
                            WHERE UserId = @uid AND Status = @approved
                            ORDER BY CreatedAt DESC;
                        ";
                        postCmd.Parameters.Add(new SqlParameter("@approved", SqlDbType.Int) { Value = (int)PostStatus.Approved });
                    }

                    postCmd.Parameters.Add(new SqlParameter("@uid", SqlDbType.NVarChar, 450) { Value = u.Id });

                    var posts = new List<object>();
                    using (var postReader = await postCmd.ExecuteReaderAsync())
                    {
                        while (await postReader.ReadAsync())
                        {
                            posts.Add(new
                            {
                                Id = postReader.GetInt32(0),
                                Content = postReader.IsDBNull(1) ? string.Empty : postReader.GetString(1),
                                ImagePath = postReader.IsDBNull(2) ? null : postReader.GetString(2),
                                CreatedAt = postReader.GetDateTime(3),
                                Status = postReader.IsDBNull(4) ? null : postReader.GetInt32(4).ToString()
                            });
                        }
                    }

                    results.Add(new
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        ProfileImage = u.ProfileImage,
                        Posts = posts
                    });
                }

                return Ok(results);
            }
}




        // -----------------------
        // UPDATE OWN PROFILE (username, email, password)
        // PUT /api/users/profile
        // -----------------------
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(uid);
            if (user == null) return NotFound("User not found.");

            // username/email updates: ensure uniqueness
            if (!string.IsNullOrWhiteSpace(dto.UserName) && !string.Equals(dto.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _userManager.FindByNameAsync(dto.UserName);
                if (exists != null) return Conflict("Username already in use.");
                user.UserName = dto.UserName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existsEmail = await _userManager.FindByEmailAsync(dto.Email);
                if (existsEmail != null) return Conflict("Email already in use.");
                user.Email = dto.Email;
                // optionally: user.EmailConfirmed = false; // depends on your flow
            }

            // apply username/email changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            // password change (if requested)
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                // require current password for safety
                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    return BadRequest("CurrentPassword is required to change password.");

                var changeResult = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
                if (!changeResult.Succeeded)
                    return BadRequest(changeResult.Errors.Select(e => e.Description));
            }

            // optional: save Bio if ApplicationUser has it (not present by default)
            // if (!string.IsNullOrWhiteSpace(dto.Bio)) user.Bio = dto.Bio; await _userManager.UpdateAsync(user);

            return Ok(new { user.Id, user.UserName, user.Email, user.ProfileImage });
        }

        // -----------------------
        // UPLOAD OWN PROFILE IMAGE
        // POST /api/users/profile/upload-image
        // -----------------------
        [Authorize]
        [HttpPost("profile/upload-image")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(uid)) return Unauthorized();

            var user = await _userManager.FindByIdAsync(uid);
            if (user == null) return NotFound("User not found.");

            if (file == null || file.Length == 0)
                return BadRequest("File is required.");

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(webRoot, "uploads", "profiles");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            // Delete old image if any
            if (!string.IsNullOrEmpty(user.ProfileImage))
            {
                var oldPath = Path.Combine(webRoot, user.ProfileImage.TrimStart('/'));
                if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
            }

            await using (var fs = System.IO.File.Create(fullPath))
            {
                await file.CopyToAsync(fs);
            }

            user.ProfileImage = $"/uploads/profiles/{fileName}";
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            return Ok(new { user.Id, user.UserName, user.Email, user.ProfileImage });
        }
    }
}
