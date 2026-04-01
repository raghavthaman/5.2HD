using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly AppDbContext _context;

    public TagsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Tag tag)
    {
        _context.Tags.Add(tag);
        await _context.SaveChangesAsync();
        return Ok(tag);
    }
}