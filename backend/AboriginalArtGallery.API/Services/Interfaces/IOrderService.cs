using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(CreateOrderDto dto, int userId);
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId);
    Task<OrderResponseDto> GetOrderByIdAsync(int id, int userId, string role);
    Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync();
    Task<OrderResponseDto> UpdateOrderStatusAsync(int id, string status);
    
    Task<object> ValidatePromoCodeAsync(string code);
    Task<IEnumerable<PromoCode>> GetPromoCodesAsync();
    Task<PromoCode> CreatePromoCodeAsync(PromoCode promoCode);
}
