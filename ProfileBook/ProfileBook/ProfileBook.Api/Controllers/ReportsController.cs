using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfileBook.Api.Data;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Controllers {
  [ApiController]
  [Route("api/[controller]")]
  public class ReportsController : ControllerBase {
    private readonly AppDbContext _db;
    public ReportsController(AppDbContext db) { _db = db; }

    [HttpPost]
    public async Task<IActionResult> Report([FromBody] Report report) {
      report.TimeStamp = DateTime.UtcNow;
      _db.Reports.Add(report);
      await _db.SaveChangesAsync();
      return Ok(report);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _db.Reports.Include(r => r.ReportedUser).Include(r => r.ReportingUser).ToListAsync());
  }
}
