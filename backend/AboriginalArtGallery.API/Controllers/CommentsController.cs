using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly IArtifactService _artifactService;

    public CommentsController(IArtifactService artifactService)
    {
        _artifactService = artifactService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentDto dto)
    {
        var comment = await _artifactService.AddCommentAsync(dto);
        return Ok(comment);
    }
}