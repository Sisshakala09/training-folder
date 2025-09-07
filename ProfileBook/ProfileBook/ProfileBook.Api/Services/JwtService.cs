using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProfileBook.Api.Models;

namespace ProfileBook.Api.Services {
  public class JwtService {
    private readonly IConfiguration _config;
    public JwtService(IConfiguration config) { _config = config; }

    public string GenerateToken(int userId, string username, string role) {
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var claims = new[] {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, username),
        new Claim(ClaimTypes.Role, role)
      };

      var token = new JwtSecurityToken(
        issuer: _config["Jwt:Issuer"],
        audience: _config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:DurationMinutes"] ?? "60")),
        signingCredentials: creds
      );

      return new JwtSecurityTokenHandler().WriteToken(token);
    }

        internal object GenerateToken(User user)
        {
            throw new NotImplementedException();
        }
    }
}
