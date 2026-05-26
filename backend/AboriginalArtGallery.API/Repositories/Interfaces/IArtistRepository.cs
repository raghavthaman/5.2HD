using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Repositories.Interfaces;

public interface IArtistRepository : IGenericRepository<Artist>
{
    Task<Artist?> GetByNameAsync(string name);
    Task<Artist?> GetArtistWithArtifactsAsync(int id);
}
