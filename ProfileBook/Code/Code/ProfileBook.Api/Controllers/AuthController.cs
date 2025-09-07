using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProfileBook.Api.Dtos;
using ProfileBook.Api.Models;
using ProfileBook.Api.Services;

namespace ProfileBook.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly IJwtService _jwt;

        public AuthController(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            IJwtService jwt)
        {
            _users = users;
            _signIn = signIn;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _users.FindByNameAsync(dto.UserName);
            if (existing != null)
                return BadRequest(new { error = "Username already taken" });

            var user = new ApplicationUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _users.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

            await _users.AddToRoleAsync(user, "User");

            var roles = await _users.GetRolesAsync(user);
            var token = _jwt.GenerateToken(user.Id, user.UserName!, user.Email ?? string.Empty, roles);

            return Ok(new { token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _users.FindByNameAsync(dto.UserNameOrEmail)
                       ?? await _users.FindByEmailAsync(dto.UserNameOrEmail);
            if (user == null)
                return Unauthorized(new { error = "Invalid credentials" });

            var ok = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!ok.Succeeded)
                return Unauthorized(new { error = "Invalid credentials" });

            var roles = await _users.GetRolesAsync(user);
            var token = _jwt.GenerateToken(user.Id, user.UserName!, user.Email ?? string.Empty, roles);

            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _users.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _users.GetRolesAsync(user);

            return Ok(new
            {
                user.Id,
                user.UserName,
                user.Email,
                roles
            });
        }
    }
}