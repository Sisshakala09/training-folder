using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureNotesApi.Data;
using SecureNotesApi.Dtos;
using SecureNotesApi.Models;
using SecureNotesApi.Services;

namespace SecureNotesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwt;

        public AuthController(AppDbContext db, JwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            dto.Username = dto.Username.Trim().ToLower();

            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest(new { message = "Username already exists." });

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { message = "User registered successfully. Please log in." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var username = dto.Username.Trim().ToLower();
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password." });

            var token = _jwt.GenerateSecurityToken(user);
            return Ok(new
            {
                token,
                expires_in = 3600,
                user = new { username = user.Username }
            });
        }
    }
}
