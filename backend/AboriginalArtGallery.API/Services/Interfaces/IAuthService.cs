using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface IAuthService
{
    Task<string> SendOtpAsync(SendOtpDto dto);
    Task<string> VerifyOtpAsync(VerifyOtpDto dto);
    Task<TokenResponseDto> LoginAsync(LoginDto dto);
}
