using System.Security.Claims;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/users")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public UsersController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public IActionResult GetUsers()
    {
        var users = _dbContext.Users.ToList();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetUser([FromRoute] int id)
    {
        var user = _dbContext.Users.Find(id);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        if (user == null)
        {
            return BadRequest();
        }
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
        user.Password = hashedPassword;

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction("GetUser", new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> UpdateUser([FromRoute] int id, User updatedUser)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != null && int.Parse(currentUserId) != id)
        {
            return Forbid();
        }

        var currentUser = await _dbContext.Users.FindAsync(id);

        if (currentUser == null)
        {
            return NotFound();
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);

        currentUser.Email = updatedUser.Email;
        currentUser.Username = updatedUser.Username;
        currentUser.Password = hashedPassword;
        currentUser.Role = updatedUser.Role;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }


    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin, user")]
    public async Task<IActionResult> DeleteUser([FromRoute] int id)
    {
        var user = await _dbContext.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }
        
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId != null && user.Id != int.Parse(currentUserId))
        {
            return Forbid();
        }
        
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private bool UserExists(int id)
    {
        return _dbContext.Users.Any(u => u.Id == id);
    }
}