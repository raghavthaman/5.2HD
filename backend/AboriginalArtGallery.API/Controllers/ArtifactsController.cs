using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Services.Interfaces;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArtifactsController : ControllerBase
{
    private readonly IArtifactService _artifactService;

    public ArtifactsController(IArtifactService artifactService)
    {
        _artifactService = artifactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 12,
        [FromQuery] string search = "",
        [FromQuery] string artType = "")
    {
        var (total, p, pSize, totalPages, items) = await _artifactService.GetPagedArtifactsAsync(page, pageSize, search, artType, Request);
        return Ok(new { total, page = p, pageSize = pSize, totalPages, items });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var artifact = await _artifactService.GetArtifactByIdAsync(id, Request);
        return Ok(artifact);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateArtifactDto dto)
    {
        var artifact = await _artifactService.CreateArtifactAsync(dto, Request);
        return CreatedAtAction(nameof(GetById), new { id = artifact.Id }, artifact);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateArtifactDto dto)
    {
        var artifact = await _artifactService.UpdateArtifactAsync(id, dto, Request);
        return Ok(artifact);
    }

    [HttpPatch("{id}/sale-status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSaleStatus(int id, [FromBody] SaleStatusDto dto)
    {
        var result = await _artifactService.UpdateSaleStatusAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        await _artifactService.DeleteArtifactAsync(id);
        return NoContent();
    }
}