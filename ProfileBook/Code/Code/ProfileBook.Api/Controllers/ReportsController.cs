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
    public class ReportsController : ControllerBase
    {
        private static readonly HashSet<string> AllowedStatuses =
            new(StringComparer.OrdinalIgnoreCase) { "Open", "Reviewing", "Actioned", "Dismissed" };

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public ReportsController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // ===== USER: create a report against another user =====
        // Body: { "reportedUserId": "<AspNetUsers.Id or UserName>", "reason": "..." }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReportDto>> Create(CreateReportDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.ReportedUserId) || string.IsNullOrWhiteSpace(dto.Reason))
                return BadRequest("reportedUserId and reason are required.");

            // Resolve the caller reliably via UserManager
            var reportingUser = await _userManager.GetUserAsync(User);
            if (reportingUser is null) return Unauthorized();

            var reportingUserId = reportingUser.Id;

            // Prevent self-reporting
            // If client provided username rather than Id, try to resolve reported user below
            if (string.Equals(dto.ReportedUserId, reportingUserId, StringComparison.Ordinal))
                return BadRequest("You cannot report yourself.");

            // Resolve reported user: try by Id first, then by UserName
            ApplicationUser? reportedUser = await _db.Users.FindAsync(dto.ReportedUserId);
            if (reportedUser == null)
            {
                reportedUser = await _db.Users.FirstOrDefaultAsync(u => u.UserName == dto.ReportedUserId);
                if (reportedUser == null)
                    return NotFound("Reported user not found.");
            }

            if (reportedUser.Id == reportingUserId)
                return BadRequest("You cannot report yourself.");

            // Optional: prevent duplicate Open report by same reporter for same user
            var existsOpen = await _db.Reports.AnyAsync(r =>
                r.ReportedUserId == reportedUser.Id &&
                r.ReportingUserId == reportingUserId &&
                r.Status == "Open");
            if (existsOpen) return BadRequest("You already have an open report for this user.");

            var report = new Report
            {
                ReportedUserId = reportedUser.Id,
                ReportingUserId = reportingUserId,
                Reason = dto.Reason.Trim(),
                Status = "Open",
                CreatedAt = DateTime.UtcNow
            };

            _db.Reports.Add(report);
            await _db.SaveChangesAsync();

            // Build DTO using the objects we already resolved (no extra DB hit)
            var dtoOut = new ReportDto(
                report.Id,
                reportedUser.Id, reportedUser.UserName ?? string.Empty,
                reportingUserId, reportingUser.UserName ?? string.Empty,
                report.Reason, report.Status, report.CreatedAt
            );

            return CreatedAtAction(nameof(GetByIdAdmin), new { id = report.Id }, dtoOut);
        }

        // ===== ADMIN: get a paged list of reports =====
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAll(
            [FromQuery] string? status = "Open",
            [FromQuery] string? search = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var q =
                from r in _db.Reports
                join repd in _db.Users on r.ReportedUserId equals repd.Id
                join repr in _db.Users on r.ReportingUserId equals repr.Id
                select new { r, repd, repr };

            if (!string.IsNullOrWhiteSpace(status))
                q = q.Where(x => x.r.Status == status);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.ToLower();
                q = q.Where(x =>
                    (x.repd.UserName ?? "").ToLower().Contains(s) ||
                    (x.repr.UserName ?? "").ToLower().Contains(s) ||
                    (x.r.Reason ?? "").ToLower().Contains(s));
            }

            var items = await q
                .OrderByDescending(x => x.r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ReportDto(
                    x.r.Id,
                    x.repd.Id, x.repd.UserName ?? "",
                    x.repr.Id, x.repr.UserName ?? "",
                    x.r.Reason, x.r.Status, x.r.CreatedAt))
                .ToListAsync();

            return Ok(items);
        }

        // ===== ADMIN: get all reports (no search / no pagination) =====
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> GetAllNoPaging()
        {
            var q =
                from r in _db.Reports
                join repd in _db.Users on r.ReportedUserId equals repd.Id
                join repr in _db.Users on r.ReportingUserId equals repr.Id
                orderby r.CreatedAt descending
                select new ReportDto(
                    r.Id,
                    repd.Id, repd.UserName ?? "",
                    repr.Id, repr.UserName ?? "",
                    r.Reason, r.Status, r.CreatedAt);

            var items = await q.ToListAsync();
            return Ok(items);
        }

        // ===== ADMIN: get one by id =====
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ReportDto>> GetByIdAdmin(int id)
        {
            var q =
                from r in _db.Reports
                where r.Id == id
                join repd in _db.Users on r.ReportedUserId equals repd.Id
                join repr in _db.Users on r.ReportingUserId equals repr.Id
                select new ReportDto(
                    r.Id,
                    repd.Id, repd.UserName ?? "",
                    repr.Id, repr.UserName ?? "",
                    r.Reason, r.Status, r.CreatedAt);

            var item = await q.FirstOrDefaultAsync();
            if (item is null) return NotFound();
            return Ok(item);
        }

        // ===== USER: my own submitted reports (optional helper) =====
        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<ReportDto>>> Mine()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is null) return Unauthorized();

            var uid = user.Id;
            var q =
                from r in _db.Reports
                where r.ReportingUserId == uid
                join repd in _db.Users on r.ReportedUserId equals repd.Id
                join repr in _db.Users on r.ReportingUserId equals repr.Id
                orderby r.CreatedAt descending
                select new ReportDto(
                    r.Id,
                    repd.Id, repd.UserName ?? "",
                    repr.Id, repr.UserName ?? "",
                    r.Reason, r.Status, r.CreatedAt);

            return Ok(await q.ToListAsync());
        }

        // ===== ADMIN: update status =====
        // Body: { "status": "Reviewing" | "Actioned" | "Dismissed" | "Open" }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateReportStatusDto dto)
        {
            if (dto is null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status is required.");

            var normalized = dto.Status.Trim();
            if (!AllowedStatuses.Contains(normalized))
                return BadRequest($"Invalid status. Allowed: {string.Join(", ", AllowedStatuses)}");

            var report = await _db.Reports.FindAsync(id);
            if (report == null) return NotFound();

            report.Status = normalized;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
