using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Repositories.Interfaces;

public interface IPromoCodeRepository : IGenericRepository<PromoCode>
{
    Task<PromoCode?> GetByCodeAsync(string code);
    Task<bool> ExistsByCodeAsync(string code);
}
