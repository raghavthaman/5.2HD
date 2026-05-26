using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Repositories.Interfaces;

public interface IArtifactRepository : IGenericRepository<Artifact>
{
    Task<(int total, IEnumerable<Artifact> items)> GetPagedArtifactsAsync(int page, int pageSize, string search, string artType);
    Task<Artifact?> GetArtifactWithDetailsAsync(int id);
    Task<List<string>> GetTagsForArtifactAsync(int artifactId);
}
