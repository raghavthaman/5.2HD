using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtifactsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ArtifactsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _context.Artifacts.Include(a => a.Artist).ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Artifact artifact)
    {
        var artist = await _context.Artists.FindAsync(artifact.ArtistId);
        if (artist == null) return BadRequest("Invalid ArtistId");

        _context.Artifacts.Add(artifact);
        await _context.SaveChangesAsync();

        return Ok(artifact);
    }
}