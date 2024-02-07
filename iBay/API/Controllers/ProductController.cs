using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Models;

namespace API.Controllers;
[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;

    public ProductsController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string sortBy,
        [FromQuery] int limit = 10)
    {
        IQueryable<Product?> query = _dbContext.Products;

        switch (sortBy)
        {
            case "date":
                query = query.OrderBy(p => p.AddedTime);
                break;
            case "type":
                query = query.OrderBy(p => p.Type);
                break;
            case "name":
                query = query.OrderBy(p => p.Name);
                break;
            case "price":
                query = query.OrderBy(p => p.Price);
                break;
            default:
                query = query.OrderBy(p => p.AddedTime);
                break;
        }

        var products = await query.Take(limit).ToListAsync();
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetProduct([FromRoute] int id)
    {
        var product = await _dbContext.Products.FindAsync(id);

        if (!ProductExists(id))
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "admin, seller")]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (product == null)
        {
            return BadRequest();
        }
        
        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction("GetProduct", new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "admin, seller")]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id, Product updatedProduct)
    {
        if (id != updatedProduct.Id)
        {
            return BadRequest();
        }

        var currentProduct = await _dbContext.Products.FindAsync(id);

        if (!ProductExists(id))
        {
            return NotFound();
        }

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (currentProduct.SellerId != int.Parse(currentUserId) && currentUserRole != "admin")
        {
            return Forbid();
        }

        currentProduct.Name = updatedProduct.Name;
        currentProduct.Image = updatedProduct.Image;
        currentProduct.Price = updatedProduct.Price;
        currentProduct.Type = updatedProduct.Type;
        currentProduct.Available = updatedProduct.Available;

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProductExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return Ok(updatedProduct + " modifié avec succès.");
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "admin, seller")]
    public async Task<IActionResult> DeleteProduct([FromRoute] int id)
    {
        if (!ProductExists(id))
        {
            return NotFound();
        }
        
        var product = await _dbContext.Products.FindAsync(id);

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (product.SellerId != int.Parse(currentUserId) && currentUserRole != "admin")
        {
            return Forbid();
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return Ok(" supprimmé avec succès.");
    }

    private bool ProductExists(int id)
    {
        return _dbContext.Products.Any(p => p != null && p.Id == id);
    }
}
