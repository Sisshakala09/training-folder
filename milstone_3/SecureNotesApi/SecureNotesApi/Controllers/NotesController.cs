using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesApi.Data;
using SecureNotesApi.Dtos;
using SecureNotesApi.Models;
using System.Security.Claims;

namespace SecureNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public NotesController(AppDbContext db) => _db = db;

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst("id").Value);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(NoteDto dto)
        {
            var note = new Note
            {
                Title = dto.Title,
                Content = dto.Content,
                UserId = GetCurrentUserId()
            };

            _db.Notes.Add(note);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Note added successfully.", noteId = note.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetNotes()
        {
            var notes = await _db.Notes
                .Where(n => n.UserId == GetCurrentUserId())
                .ToListAsync();

            return Ok(notes);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, NoteDto dto)
        {
            var note = await _db.Notes.SingleOrDefaultAsync(n => n.Id == id && n.UserId == GetCurrentUserId());
            if (note == null) return NotFound();

            note.Title = dto.Title;
            note.Content = dto.Content;
            await _db.SaveChangesAsync();

            return Ok(new { message = "Note updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _db.Notes.SingleOrDefaultAsync(n => n.Id == id && n.UserId == GetCurrentUserId());
            if (note == null) return NotFound();

            _db.Notes.Remove(note);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Note deleted successfully." });
        }
    }
}
