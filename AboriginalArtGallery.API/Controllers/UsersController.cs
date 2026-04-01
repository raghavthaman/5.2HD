using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;

    public UsersController(AppDbContext context)
    {
        _context = context;
    }

    // GET all users
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users.ToListAsync();
        return Ok(users);
    }

    // GET user by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");

        return Ok(user);
    }

    // UPDATE user
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.Role = updatedUser.Role;

        await _context.SaveChangesAsync();

        return Ok(user);
    }

    // DELETE user
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok("User deleted successfully");
    }
}