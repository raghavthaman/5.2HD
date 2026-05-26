using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface IArtistService
{
    Task<IEnumerable<ArtistResponseDto>> GetAllArtistsAsync();
    Task<ArtistResponseDto> GetArtistByIdAsync(int id, HttpRequest request);
    Task<Artist> CreateArtistAsync(Artist artist);
    Task<Artist> UpdateArtistAsync(int id, Artist artist);
    Task DeleteArtistAsync(int id);
}
