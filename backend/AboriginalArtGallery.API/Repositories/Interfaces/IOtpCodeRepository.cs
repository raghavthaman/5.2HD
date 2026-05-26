using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Repositories.Interfaces;

public interface IOtpCodeRepository : IGenericRepository<OtpCode>
{
    Task<OtpCode?> GetActiveOtpAsync(string email, string code);
    Task<OtpCode?> GetActivePayloadAsync(string email);
    Task InvalidatePreviousOtpsAsync(string email);
}
