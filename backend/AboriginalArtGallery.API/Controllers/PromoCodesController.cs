using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Services.Interfaces;
using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PromoCodesController : ControllerBase
{
    private readonly IOrderService _orderService;

    public PromoCodesController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPromoCodes()
    {
        var promoCodes = await _orderService.GetPromoCodesAsync();
        return Ok(promoCodes);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePromoCode([FromBody] PromoCode promoCode)
    {
        var created = await _orderService.CreatePromoCodeAsync(promoCode);
        return CreatedAtAction(nameof(GetPromoCodes), new { id = created.Id }, created);
    }

    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> ValidatePromoCode([FromBody] ValidatePromoDto dto)
    {
        var result = await _orderService.ValidatePromoCodeAsync(dto.Code);
        return Ok(result);
    }
}
