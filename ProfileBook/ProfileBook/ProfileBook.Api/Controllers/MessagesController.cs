using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class MessagesController : ControllerBase {
    private readonly AppDbContext _db;
    public MessagesController(AppDbContext db) { _db = db; }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] Message message) {
      message.TimeStamp = DateTime.UtcNow;
      _db.Messages.Add(message);
      await _db.SaveChangesAsync();
      return Ok(message);
    }

    [HttpGet("conv")]
    public async Task<IActionResult> Conversation(int userA, int userB) {
      var msgs = await _db.Messages
        .Where(m => (m.SenderId == userA && m.ReceiverId == userB) || (m.SenderId == userB && m.ReceiverId == userA))
        .OrderBy(m => m.TimeStamp).ToListAsync();
      return Ok(msgs);
    }
  }
}
