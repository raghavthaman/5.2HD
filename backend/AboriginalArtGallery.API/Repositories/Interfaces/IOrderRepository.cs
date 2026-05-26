using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Repositories.Interfaces;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(int id);
    Task<IEnumerable<Order>> GetMyOrdersWithItemsAsync(int userId);
    Task<IEnumerable<Order>> GetAllOrdersWithDetailsAsync();
}
