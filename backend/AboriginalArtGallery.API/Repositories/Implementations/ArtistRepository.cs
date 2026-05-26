using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;

namespace AboriginalArtGallery.API.Repositories.Implementations;

public class ArtistRepository : GenericRepository<Artist>, IArtistRepository
{
    public ArtistRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Artist?> GetByNameAsync(string name)
    {
        var nameLower = name.Trim().ToLower();
        return await _dbSet.FirstOrDefaultAsync(a => a.Name.ToLower() == nameLower);
    }

    public async Task<Artist?> GetArtistWithArtifactsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Artifacts)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
