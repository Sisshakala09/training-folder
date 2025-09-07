using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Dtos;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public MessagesController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // POST /api/messages -> send a new message
        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
        {
            if (dto is null)
                return BadRequest("Request body is required.");

            if (string.IsNullOrWhiteSpace(dto.ReceiverId) || string.IsNullOrWhiteSpace(dto.Body))
                return BadRequest("receiverId and body are required.");

            // Get the authenticated identity user (this returns the AspNetUsers.Id value)
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var senderId = user.Id;

            // Ensure receiver exists. If client passed username accidentally, try to resolve it.
            var receiver = await _db.Users.FindAsync(dto.ReceiverId);
            if (receiver == null)
            {
                // try resolve by username
                var byName = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.ReceiverId);
                if (byName != null)
                {
                    dto = dto with { ReceiverId = byName.Id }; // requires DTO is a record
                    receiver = byName;
                }
                else
                {
                    return BadRequest("Receiver not found.");
                }
            }

            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = dto.ReceiverId,
                Body = dto.Body,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _db.Messages.Add(msg);
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Provide a helpful error for debugging; in production log this instead.
                return Problem(detail: ex.InnerException?.Message ?? ex.Message, statusCode: 500);
            }

            // Return created message DTO with sender/receiver info
            var senderUser = await _db.Users.AsNoTracking().Where(u => u.Id == msg.SenderId)
                .Select(u => new { u.Id, u.UserName, u.ProfileImage }).FirstOrDefaultAsync();
            var receiverUser = await _db.Users.AsNoTracking().Where(u => u.Id == msg.ReceiverId)
                .Select(u => new { u.Id, u.UserName, u.ProfileImage }).FirstOrDefaultAsync();

            UserDto? mapUser(dynamic? u) => u is null ? null : new UserDto(u.Id, u.UserName, GetAbsoluteUrl(u.ProfileImage));

            var dtoOut = new MessageDto(msg.Id, msg.SenderId, msg.ReceiverId, msg.Body, msg.SentAt,
                                        Sender: mapUser(senderUser), Receiver: mapUser(receiverUser));
            return Ok(dtoOut);
        }

        // GET /api/messages -> list all conversations with last message + unread count
        [HttpGet]
        public async Task<IActionResult> Conversations()
        {
            try
            {
                var myId = (await _userManager.GetUserAsync(User))?.Id ?? User.FindFirst("uid")?.Value ?? User.Identity?.Name!;
                if (string.IsNullOrEmpty(myId)) return Unauthorized();

                // 1) Materialize all messages involving me
                var messages = await _db.Messages
                    .AsNoTracking()
                    .Where(m => m.SenderId == myId || m.ReceiverId == myId)
                    .ToListAsync();

                // 2) Materialize all users (we'll filter in-memory)
                var allUsers = await _db.Users
                    .AsNoTracking()
                    .Select(u => new { u.Id, u.UserName, u.Email, u.ProfileImage })
                    .ToListAsync();

                // 3) Find distinct other user ids
                var otherIds = messages
                    .Select(m => m.SenderId == myId ? m.ReceiverId : m.SenderId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .Distinct()
                    .ToList();

                if (!otherIds.Any())
                    return Ok(Array.Empty<ConversationSummaryDto>());

                // 4) Last message from each contact to me
                var lastFromDict = messages
                    .Where(m => m.ReceiverId == myId)
                    .GroupBy(m => m.SenderId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.SentAt).First());

                // 5) Last overall message between me and each contact
                var lastOverallDict = messages
                    .GroupBy(m => m.SenderId == myId ? m.ReceiverId : m.SenderId)
                    .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.SentAt).First());

                // 6) Unread counts per sender
                var unreadDict = messages
                    .Where(m => m.ReceiverId == myId && !m.IsRead)
                    .GroupBy(m => m.SenderId)
                    .ToDictionary(g => g.Key, g => g.Count());

                // 7) Build response using in-memory lookups
                var result = otherIds.Select(otherId =>
                {
                    var u = allUsers.FirstOrDefault(x => x.Id == otherId);
                    lastFromDict.TryGetValue(otherId, out var lastFromMsg);
                    lastOverallDict.TryGetValue(otherId, out var lastOverallMsg);
                    unreadDict.TryGetValue(otherId, out var unread);

                        MessageDto? map(Message? m) =>
                        m is null ? null : new MessageDto(m.Id, m.SenderId, m.ReceiverId, m.Body, m.SentAt,
                                                          Sender: allUsers.FirstOrDefault(x => x.Id == m.SenderId) is var su ? new UserDto(su?.Id ?? string.Empty, su?.UserName, GetAbsoluteUrl(su?.ProfileImage)) : null,
                                                          Receiver: allUsers.FirstOrDefault(x => x.Id == m.ReceiverId) is var ru ? new UserDto(ru?.Id ?? string.Empty, ru?.UserName, GetAbsoluteUrl(ru?.ProfileImage)) : null);

                    return new ConversationSummaryDto(
                        UserId: otherId,
                        UserName: u?.UserName ?? string.Empty,
                        Email: u?.Email,
                        LastMessageFromUserToMe: map(lastFromMsg),
                        LastMessageOverall: map(lastOverallMsg),
                        UnreadCount: unread
                    );
                })
                .OrderByDescending(x => x.LastMessageOverall?.SentAt ?? DateTime.MinValue)
                .ToList();

                return Ok(result);
            }
            catch (Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                return Problem(detail: sqlEx.ToString(), statusCode: 500);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.ToString(), statusCode: 500);
            }
        }

        // GET /api/messages/{userId} -> chat history with one user
        [HttpGet("{userId}")]
        public async Task<IActionResult> History(string userId)
        {
            var myId = (await _userManager.GetUserAsync(User))?.Id ?? User.FindFirst("uid")?.Value ?? User.Identity?.Name!;
            if (string.IsNullOrEmpty(myId)) return Unauthorized();

            var itemsRaw = await _db.Messages
                .AsNoTracking()
                .Where(m => (m.SenderId == myId && m.ReceiverId == userId) ||
                            (m.SenderId == userId && m.ReceiverId == myId))
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    m.Body,
                    m.SentAt,
                    SenderUserName = m.Sender != null ? m.Sender.UserName : null,
                    SenderProfileImage = m.Sender != null ? m.Sender.ProfileImage : null,
                    ReceiverUserName = m.Receiver != null ? m.Receiver.UserName : null,
                    ReceiverProfileImage = m.Receiver != null ? m.Receiver.ProfileImage : null
                })
                .ToListAsync();

            var items = itemsRaw.Select(m => new MessageDto(
                    m.Id,
                    m.SenderId,
                    m.ReceiverId,
                    m.Body,
                    m.SentAt,
                    Sender: new UserDto(m.SenderId, m.SenderUserName, GetAbsoluteUrl(m.SenderProfileImage)),
                    Receiver: new UserDto(m.ReceiverId, m.ReceiverUserName, GetAbsoluteUrl(m.ReceiverProfileImage))
                ))
                .ToList();

            return Ok(items);
        }

        // Helper to create absolute URL for profile image paths (copied pattern from PostsController)
        private string? GetAbsoluteUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return null;
            var path = relativePath.StartsWith("/") ? relativePath : "/" + relativePath;
            return $"{Request.Scheme}://{Request.Host}{path}";
        }
    }
}
