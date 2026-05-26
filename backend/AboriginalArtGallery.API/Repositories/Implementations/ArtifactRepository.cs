using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.Repositories.Interfaces;

namespace AboriginalArtGallery.API.Repositories.Implementations;

public class ArtifactRepository : GenericRepository<Artifact>, IArtifactRepository
{
    public ArtifactRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(int total, IEnumerable<Artifact> items)> GetPagedArtifactsAsync(
        int page, int pageSize, string search, string artType)
    {
        var query = _dbSet.Include(a => a.Artist).AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            var searchLower = search.ToLower();
            query = query.Where(a => a.Title.ToLower().Contains(searchLower));
        }

        if (!string.IsNullOrEmpty(artType))
        {
            var typeLower = artType.ToLower();
            query = query.Where(a => a.ArtType != null && a.ArtType.ToLower().Contains(typeLower));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (total, items);
    }

    public async Task<Artifact?> GetArtifactWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(a => a.Artist)
            .Include(a => a.Comments)
                .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<string>> GetTagsForArtifactAsync(int artifactId)
    {
        return await _dbContext.ArtifactTags
            .Where(at => at.ArtifactId == artifactId)
            .Select(at => at.Tag!.Name)
            .ToListAsync();
    }
}
