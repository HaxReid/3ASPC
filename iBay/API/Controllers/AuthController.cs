using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public AuthController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest loginRequest)
    {
        var user = _dbContext.Users.SingleOrDefault(u => u.Email == loginRequest.Email);

        if (user == null)
        {
            return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(loginRequest.Password);
        if (hashedPassword == user.Password)
        {
            var token = GenerateJwtToken(user.Id, user.Role);
            return Ok(new { Token = token });
        }
        return Unauthorized("Nom d'utilisateur ou mot de passe incorrect.");
    }


    private string GenerateJwtToken(int userId, string userRole)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("secret");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, userRole)
            }),
            Expires = DateTime.UtcNow.AddHours(1), 
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
