using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Services.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otp);
    Task SendOrderConfirmationAsync(string toEmail, string username, Order order);
}
