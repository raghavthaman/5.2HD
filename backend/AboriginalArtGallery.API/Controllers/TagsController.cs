using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagsController : ControllerBase
{
    private readonly IArtifactService _artifactService;

    public TagsController(IArtifactService artifactService)
    {
        _artifactService = artifactService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Tag tag)
    {
        var created = await _artifactService.AddTagAsync(tag);
        return Ok(created);
    }
}