using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using API.Models;

[Route("api/cart")]
[ApiController]
[Authorize] 
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public CartController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> AddToCart(int id, [FromRoute] int productId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var existingCartItem = await _dbContext.Carts
            .FirstOrDefaultAsync(c => currentUserId != null && c.UserId == int.Parse(currentUserId) && c.ProductId == productId) ;

        if (existingCartItem != null)
        {
            return BadRequest("Le product est déjà dans le cart.");
        }

        if (currentUserId != null)
        {
            var cartItem = new Cart
            {
                UserId = int.Parse(currentUserId),
                ProductId = productId
            };

            _dbContext.Carts.Add(cartItem);
        }

        await _dbContext.SaveChangesAsync();

        return Ok("Produit ajouté au panier avec succès.");
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> RemoveFromCart([FromRoute] int productId)
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var cartItemToRemove = await _dbContext.Carts
            .FirstOrDefaultAsync(c => currentUserId != null && c.UserId == int.Parse(currentUserId) && c.ProductId == productId);

        if (cartItemToRemove == null)
        {
            return NotFound("Le produit n'est pas dans le panier.");
        }

        _dbContext.Carts.Remove(cartItemToRemove);
        await _dbContext.SaveChangesAsync();

        return Ok("Produit supprimé du panier avec succès.");
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var cartItems = await _dbContext.Carts
            .Where(c => currentUserId != null && c.UserId == int.Parse(currentUserId))
            .ToListAsync();

        return Ok(cartItems);
    }
}
