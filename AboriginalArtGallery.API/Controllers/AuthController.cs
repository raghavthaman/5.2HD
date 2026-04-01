using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    // 🔷 REGISTER
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        // check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest("User already exists");
        }

        // hash password
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // ✅ IMPORTANT FIX (for test)
        return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
    }

    // 🔷 LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

        // verify password
        bool isValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

        if (!isValid)
        {
            // ✅ IMPORTANT FIX (for test)
            return Unauthorized("Invalid credentials");
        }

        // create JWT token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(
                Convert.ToDouble(_config["JwtSettings:ExpiryHours"])
            ),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new { token = jwt });
    }
}