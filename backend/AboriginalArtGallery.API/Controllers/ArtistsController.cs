using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtistsController : ControllerBase
{
    private readonly IArtistService _artistService;

    public ArtistsController(IArtistService artistService)
    {
        _artistService = artistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var artists = await _artistService.GetAllArtistsAsync();
        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var artist = await _artistService.GetArtistByIdAsync(id, Request);
        return Ok(artist);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Artist artist)
    {
        var created = await _artistService.CreateArtistAsync(artist);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Artist updatedArtist)
    {
        var artist = await _artistService.UpdateArtistAsync(id, updatedArtist);
        return Ok(artist);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _artistService.DeleteArtistAsync(id);
        return NoContent();
    }
}