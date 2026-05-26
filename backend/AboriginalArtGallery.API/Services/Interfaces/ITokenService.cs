using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface ITokenService
{
    string GenerateToken(User user);
}
