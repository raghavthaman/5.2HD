using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;

namespace AboriginalArtGallery.API.Repositories.Implementations;

public class OtpCodeRepository : GenericRepository<OtpCode>, IOtpCodeRepository
{
    public OtpCodeRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<OtpCode?> GetActiveOtpAsync(string email, string code)
    {
        return await _dbSet.FirstOrDefaultAsync(o => o.Email == email && o.Code == code && !o.IsUsed);
    }

    public async Task<OtpCode?> GetActivePayloadAsync(string email)
    {
        var payloadKey = $"__payload__{email}";
        return await _dbSet.FirstOrDefaultAsync(o => o.Email == payloadKey && !o.IsUsed);
    }

    public async Task InvalidatePreviousOtpsAsync(string email)
    {
        var previousOtps = await _dbSet.Where(o => (o.Email == email || o.Email == $"__payload__{email}") && !o.IsUsed).ToListAsync();
        _dbSet.RemoveRange(previousOtps);
    }
}
