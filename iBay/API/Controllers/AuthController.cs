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
        try
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == loginRequest.Email);

            if (user == null)
            {
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect. 3 ");
            }

            bool isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.Password);

            if (isPasswordCorrect)
            {
                var token = GenerateJwtToken(user.Id, user.Role);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized("Nom d'utilisateur ou mot de passe incorrect 2 .");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Une erreur s'est produite lors de l'authentification : {ex.Message}");
        }
    }



    private string GenerateJwtToken(int userId, string userRole)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("this_variable_is_my_secret_connard_256_allumez_le_feu_au_piano_cest_la_fete_au_village_attention_demain");
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
