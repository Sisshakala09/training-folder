using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Models;
using ProfileBook.Api.Dtos;

namespace ProfileBook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // authenticated; admin check is performed inside methods
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _env = env;
        }

        // -----------------------
        // Helper: check Admin role (DB-backed)
        // -----------------------
        private async Task<bool> IsAdminUserAsync()
        {
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("uid")?.Value;
            if (string.IsNullOrEmpty(uid)) return false;

            var caller = await _userManager.FindByIdAsync(uid);
            if (caller == null) return false;

            return await _userManager.IsInRoleAsync(caller, "Admin");
        }

        // -----------------------
        // Create user with profile image (multipart/form-data)
        // POST /api/admin/users/create-with-profile
        // POST /api/admin/users/create-with-profile
        [HttpPost("users/create-with-profile")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> CreateUserWithProfile([FromForm] CreateUserWithProfileFormDto dto)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            if (dto is null) return BadRequest("Form required.");
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("userName and password are required.");

            if (await _userManager.FindByNameAsync(dto.UserName) != null)
                return Conflict("UserName already exists.");

            if (!string.IsNullOrWhiteSpace(dto.Email) && await _userManager.FindByEmailAsync(dto.Email) != null)
                return Conflict("Email already exists.");

            // Save profile image
            string? savedRelPath = null;
            if (dto.ProfileImage is { Length: > 0 })
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var folder = Path.Combine(webRoot, "uploads", "profiles");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImage.FileName)}";
                var fullPath = Path.Combine(folder, fileName);

                await using (var fs = System.IO.File.Create(fullPath))
                {
                    await dto.ProfileImage.CopyToAsync(fs);
                }

                savedRelPath = $"/uploads/profiles/{fileName}";
            }

            var user = new ApplicationUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                EmailConfirmed = dto.EmailConfirmed,
                CreatedAt = DateTime.UtcNow,
                ProfileImage = savedRelPath
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                if (savedRelPath != null)
                {
                    try
                    {
                        var deletePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", savedRelPath.TrimStart('/'));
                        if (System.IO.File.Exists(deletePath)) System.IO.File.Delete(deletePath);
                    }
                    catch { }
                }

                return BadRequest(createResult.Errors.Select(e => e.Description));
            }

            // assign roles if provided (comma-separated)
            if (!string.IsNullOrWhiteSpace(dto.Roles))
            {
                var rolesArr = dto.Roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                foreach (var r in rolesArr)
                    if (!await _roleManager.RoleExistsAsync(r))
                        await _roleManager.CreateAsync(new IdentityRole(r));

                var addRolesRes = await _userManager.AddToRolesAsync(user, rolesArr);
                if (!addRolesRes.Succeeded)
                    return BadRequest(addRolesRes.Errors.Select(e => e.Description));
            }

            var assignedRoles = (await _userManager.GetRolesAsync(user)).ToArray();
            var outDto = new UserAdminDto(user.Id, user.UserName ?? "", user.Email ?? "", assignedRoles, user.ProfileImage);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, outDto);
        }

        // GET /api/admin/users
        // Admin-only: list users (id, username, email, profileImage)
        // Optional paging: ?page=1&pageSize=50
        [HttpGet("users")]
        public async Task<IActionResult> ListUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 500) pageSize = 50;

            var q = _db.Users
                .AsNoTracking()
                .OrderBy(u => u.UserName) // stable order
                .Select(u => new
                {
                    u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    ProfileImage = u.ProfileImage
                });

            var total = await q.CountAsync();
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new
            {
                Total = total,
                Page = page,
                PageSize = pageSize,
                Items = items
            };

            return Ok(result);
        }

        // -----------------------
        // Get user (admin)
        // GET /api/admin/users/{id}
        // -----------------------
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();

            var roles = (await _userManager.GetRolesAsync(u)).ToArray();
            return Ok(new UserAdminDto(u.Id, u.UserName ?? "", u.Email ?? "", roles, u.ProfileImage));
        }

        // -----------------------
        // Update user (username, email, password, roles)
        // PUT /api/admin/users/{id}
        [HttpPut("users/{id}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> UpdateUserWithProfile(string id, [FromForm] UpdateUserWithProfileFormDto dto)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            // update username & email
            if (!string.IsNullOrWhiteSpace(dto.UserName) && !string.Equals(dto.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.FindByNameAsync(dto.UserName) != null)
                    return Conflict("UserName already exists.");
                user.UserName = dto.UserName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return Conflict("Email already exists.");
                user.Email = dto.Email;
            }

            // update profile picture
            if (dto.ProfileImage is { Length: > 0 })
            {
                // delete old
                if (!string.IsNullOrEmpty(user.ProfileImage))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Delete(oldPath);
                }

                // save new
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var folder = Path.Combine(webRoot, "uploads", "profiles");
                Directory.CreateDirectory(folder);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.ProfileImage.FileName)}";
                var fullPath = Path.Combine(folder, fileName);

                await using (var fs = System.IO.File.Create(fullPath))
                {
                    await dto.ProfileImage.CopyToAsync(fs);
                }

                user.ProfileImage = $"/uploads/profiles/{fileName}";
            }

            // apply changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            // update password
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.Password);
                if (!passResult.Succeeded)
                    return BadRequest(passResult.Errors.Select(e => e.Description));
            }

            // update roles
            if (!string.IsNullOrWhiteSpace(dto.Roles))
            {
                var newRoles = dto.Roles
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                foreach (var r in newRoles)
                    if (!await _roleManager.RoleExistsAsync(r))
                        await _roleManager.CreateAsync(new IdentityRole(r));

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                    return BadRequest(removeRes.Errors.Select(e => e.Description));

                var addRes = await _userManager.AddToRolesAsync(user, newRoles);
                if (!addRes.Succeeded)
                    return BadRequest(addRes.Errors.Select(e => e.Description));
            }

            var rolesFinal = (await _userManager.GetRolesAsync(user)).ToArray();
            var outDto = new UserAdminDto(user.Id, user.UserName ?? "", user.Email ?? "", rolesFinal, user.ProfileImage);

            return Ok(outDto);
        }

        // -----------------------
        // Partial update (JSON) - update single or multiple fields (no profile image)
        // PATCH /api/admin/users/{id}
        [HttpPatch("users/{id}")]
        public async Task<IActionResult> PatchUser(string id, [FromBody] UpdateUserDto dto)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            if (dto == null) return BadRequest("Request body required.");

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            // update username & email only if provided
            if (!string.IsNullOrWhiteSpace(dto.UserName) && !string.Equals(dto.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.FindByNameAsync(dto.UserName) != null)
                    return Conflict("UserName already exists.");
                user.UserName = dto.UserName;
            }

            if (!string.IsNullOrWhiteSpace(dto.Email) && !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                if (await _userManager.FindByEmailAsync(dto.Email) != null)
                    return Conflict("Email already exists.");
                user.Email = dto.Email;
            }

            // apply user updates (username/email)
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors.Select(e => e.Description));

            // update password if requested
            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
                if (!passResult.Succeeded)
                    return BadRequest(passResult.Errors.Select(e => e.Description));
            }

            // update roles if provided (null means ignore; empty array will clear roles)
            if (dto.Roles != null)
            {
                var newRoles = dto.Roles
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

                foreach (var r in newRoles)
                    if (!await _roleManager.RoleExistsAsync(r))
                        await _roleManager.CreateAsync(new IdentityRole(r));

                var currentRoles = await _userManager.GetRolesAsync(user);
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                    return BadRequest(removeRes.Errors.Select(e => e.Description));

                if (newRoles.Length > 0)
                {
                    var addRes = await _userManager.AddToRolesAsync(user, newRoles);
                    if (!addRes.Succeeded)
                        return BadRequest(addRes.Errors.Select(e => e.Description));
                }
            }

            var rolesFinal = (await _userManager.GetRolesAsync(user)).ToArray();
            var outDto2 = new UserAdminDto(user.Id, user.UserName ?? "", user.Email ?? "", rolesFinal, user.ProfileImage);

            return Ok(outDto2);
        }


        // -----------------------
        // Delete user
        // DELETE /api/admin/users/{id}
        // -----------------------
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // remove dependent entities that reference this user to avoid FK constraint failures
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Group memberships
                var memberships = await _db.GroupMembers.Where(gm => gm.UserId == id).ToListAsync();
                if (memberships.Count > 0)
                {
                    _db.GroupMembers.RemoveRange(memberships);
                }

                // Messages where user is sender or receiver
                var messages = await _db.Messages.Where(m => m.SenderId == id || m.ReceiverId == id).ToListAsync();
                if (messages.Count > 0)
                {
                    _db.Messages.RemoveRange(messages);
                }

                // PostLikes by this user
                var postLikes = await _db.PostLikes.Where(pl => pl.UserId == id).ToListAsync();
                if (postLikes.Count > 0)
                {
                    _db.PostLikes.RemoveRange(postLikes);
                }

                // Comments by this user
                var comments = await _db.Comments.Where(c => c.UserId == id).ToListAsync();
                if (comments.Count > 0)
                {
                    _db.Comments.RemoveRange(comments);
                }

                // Posts authored by this user: delete likes/comments on them first, then posts
                var postIds = await _db.Posts.Where(p => p.UserId == id).Select(p => p.Id).ToListAsync();
                if (postIds.Count > 0)
                {
                    var likesOnPosts = await _db.PostLikes.Where(pl => postIds.Contains(pl.PostId)).ToListAsync();
                    if (likesOnPosts.Count > 0) _db.PostLikes.RemoveRange(likesOnPosts);

                    var commentsOnPosts = await _db.Comments.Where(c => postIds.Contains(c.PostId)).ToListAsync();
                    if (commentsOnPosts.Count > 0) _db.Comments.RemoveRange(commentsOnPosts);

                    var posts = await _db.Posts.Where(p => p.UserId == id).ToListAsync();
                    if (posts.Count > 0) _db.Posts.RemoveRange(posts);
                }

                // Reports referencing this user (as reported or reporter)
                var reports = await _db.Reports.Where(r => r.ReportedUserId == id || r.ReportingUserId == id).ToListAsync();
                if (reports.Count > 0)
                {
                    _db.Reports.RemoveRange(reports);
                }

                await _db.SaveChangesAsync();

                var delRes = await _userManager.DeleteAsync(user);
                if (!delRes.Succeeded)
                {
                    await tx.RollbackAsync();
                    return BadRequest(delRes.Errors.Select(e => e.Description));
                }

                await tx.CommitAsync();
                return NoContent();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        // -----------------------
        // Create group and optionally add members
        // POST /api/admin/groups
        [HttpPost("groups")]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupDto dto)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            if (dto == null || string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Group name is required.");

            // Normalize to array (avoid null)
            var memberIds = dto.MemberIds ?? Array.Empty<string>();

            // Create group
            var group = new Group { Name = dto.Name.Trim() };

            // If there are any memberIds, validate them one-by-one and add valid ones.
            // This avoids building a single EF 'IN (...)' query which was causing the 'WITH' SQL error.
            if (memberIds.Length > 0)
            {
                // Use a HashSet to avoid duplicate adds
                var distinctIds = new HashSet<string>(memberIds.Where(id => !string.IsNullOrWhiteSpace(id)));

                foreach (var uid in distinctIds)
                {
                    // Small, targeted check per id — avoids problematic EF translation
                    var exists = await _db.Users.AnyAsync(u => u.Id == uid);
                    if (exists)
                    {
                        group.Members.Add(new GroupMember { UserId = uid });
                    }
                    // else: silently skip invalid ids. If you want, collect and report them after the loop.
                }
            }

            _db.Groups.Add(group);
            await _db.SaveChangesAsync();

            var dtoOut = new
            {
                group.Id,
                group.Name,
                Members = group.Members.Select(m => m.UserId).ToList()
            };

            return CreatedAtAction(nameof(GetGroup), new { id = group.Id }, dtoOut);
        }




        // helper GET to fetch group
        [HttpGet("groups/{id}")]
        public async Task<IActionResult> GetGroup(int id)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var group = await _db.Groups
                .Include(g => g.Members)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null) return NotFound();

            // build members list by looking up each member individually via UserManager.
            // This avoids generating complex IN/OPENJSON SQL that some SQL Server versions reject.
            var membersDto = new List<UserAdminDto>();
            foreach (var gm in group.Members)
            {
                if (string.IsNullOrWhiteSpace(gm.UserId)) continue;
                var u = await _userManager.FindByIdAsync(gm.UserId);
                if (u == null) continue;
                var roles = (await _userManager.GetRolesAsync(u)).ToArray();
                membersDto.Add(new UserAdminDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty, roles, u.ProfileImage));
            }

            var dtoOut = new
            {
                group.Id,
                group.Name,
                Members = membersDto
            };

            return Ok(dtoOut);
        }

        // -----------------------
        // List all groups with member details (admin-only)
        // GET /api/admin/groups
        [HttpGet("groups")]
        public async Task<IActionResult> ListGroups()
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var groups = await _db.Groups
                .Include(g => g.Members)
                .ToListAsync();

            var outList = new List<object>();
            foreach (var group in groups)
            {
                var membersDto = new List<UserAdminDto>();
                foreach (var gm in group.Members)
                {
                    if (string.IsNullOrWhiteSpace(gm.UserId)) continue;
                    var u = await _userManager.FindByIdAsync(gm.UserId);
                    if (u == null) continue;
                    var roles = (await _userManager.GetRolesAsync(u)).ToArray();
                    membersDto.Add(new UserAdminDto(u.Id, u.UserName ?? string.Empty, u.Email ?? string.Empty, roles, u.ProfileImage));
                }

                outList.Add(new { group.Id, group.Name, Members = membersDto });
            }

            return Ok(outList);
        }


    

        // -----------------------
        // Add user to group
        // POST /api/admin/groups/{id}/add/{userId}
        // -----------------------
        [HttpPost("groups/{id}/add/{userId}")]
        public async Task<IActionResult> AddUserToGroup(int id, string userId)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var g = await _db.Groups.FindAsync(id);
            if (g == null) return NotFound("Group not found");

            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found");

            var exists = await _db.GroupMembers.AnyAsync(m => m.GroupId == id && m.UserId == userId);
            if (exists) return BadRequest("User already in group");

            _db.GroupMembers.Add(new GroupMember { GroupId = id, UserId = userId });
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // -----------------------
        // Remove user from group
        // POST /api/admin/groups/{id}/remove/{userId}
        // -----------------------
        [HttpPost("groups/{id}/remove/{userId}")]
        public async Task<IActionResult> RemoveUserFromGroup(int id, string userId)
        {
            if (!await IsAdminUserAsync()) return Forbid();

            var gm = await _db.GroupMembers.FindAsync(id, userId);
            if (gm == null) return NotFound();

            _db.GroupMembers.Remove(gm);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
