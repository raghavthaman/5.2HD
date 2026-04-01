using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArtistsController(AppDbContext context)
    {
        _context = context;
    }

    // GET all artists
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Artists.ToListAsync());
    }

    // GET artist by id
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
            return NotFound("Artist not found"); // ✅ FIXED

        return Ok(artist);
    }

    // CREATE artist
    [HttpPost]
    public async Task<IActionResult> Create(Artist artist)
    {
        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        // ✅ FIXED (important for test)
        return CreatedAtAction(nameof(GetById), new { id = artist.Id }, artist);
    }

    // UPDATE artist
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Artist updatedArtist)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
            return NotFound("Artist not found"); // optional improvement

        artist.Name = updatedArtist.Name;
        artist.Tribe = updatedArtist.Tribe;
        artist.Biography = updatedArtist.Biography;
        artist.BirthYear = updatedArtist.BirthYear;

        await _context.SaveChangesAsync();

        return Ok(artist);
    }

    // DELETE artist
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
            return NotFound("Artist not found");

        _context.Artists.Remove(artist);
        await _context.SaveChangesAsync();

        // ✅ FIXED (important for test)
        return NoContent();
    }
}