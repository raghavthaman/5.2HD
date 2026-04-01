using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public CommentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Comment comment)
    {
        var user = await _context.Users.FindAsync(comment.UserId);
        var artifact = await _context.Artifacts.FindAsync(comment.ArtifactId);

        if (user == null || artifact == null)
            return BadRequest("Invalid UserId or ArtifactId");

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return Ok(comment);
    }
}