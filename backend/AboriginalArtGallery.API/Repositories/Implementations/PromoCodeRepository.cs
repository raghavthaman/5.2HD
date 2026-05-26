using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;

namespace AboriginalArtGallery.API.Repositories.Implementations;

public class PromoCodeRepository : GenericRepository<PromoCode>, IPromoCodeRepository
{
    public PromoCodeRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PromoCode?> GetByCodeAsync(string code)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.Code == code);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        return await _dbSet.AnyAsync(p => p.Code == code);
    }
}
