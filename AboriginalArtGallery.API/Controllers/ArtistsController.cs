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
            return NotFound("Artist not found");

        return Ok(artist);
    }

    // CREATE artist
    [HttpPost]
    public async Task<IActionResult> Create(Artist artist)
    {
        if (string.IsNullOrWhiteSpace(artist.Name))
            return BadRequest("Artist name is required");

        var trimmedName = artist.Name.Trim();

        var existingArtist = await _context.Artists
            .FirstOrDefaultAsync(a => a.Name.ToLower() == trimmedName.ToLower());

        if (existingArtist != null)
            return Conflict("Artist with the same name already exists");

        artist.Name = trimmedName;

        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = artist.Id }, artist);
    }

    // UPDATE artist
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Artist updatedArtist)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist == null)
            return NotFound("Artist not found");

        if (string.IsNullOrWhiteSpace(updatedArtist.Name))
            return BadRequest("Artist name is required");

        var trimmedName = updatedArtist.Name.Trim();

        var duplicateArtist = await _context.Artists
            .FirstOrDefaultAsync(a => a.Id != id && a.Name.ToLower() == trimmedName.ToLower());

        if (duplicateArtist != null)
            return Conflict("Another artist with the same name already exists");

        artist.Name = trimmedName;
        artist.Tribe = updatedArtist.Tribe;
        artist.Biography = updatedArtist.Biography;
        artist.BirthYear = updatedArtist.BirthYear;
        artist.Country = updatedArtist.Country;

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

        return NoContent();
    }
}