using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using AboriginalArtGallery.API.Controllers;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Services.Interfaces;
using System.Threading.Tasks;

namespace AboriginalArtGallery.Tests;

public class AuthControllerTests
{
    [Fact]
    public async Task SendOtp_ReturnsOk_WithSuccessMessage()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.SendOtpAsync(It.IsAny<SendOtpDto>())).ReturnsAsync("OTP sent successfully");

        var controller = new AuthController(mockService.Object);
        var dto = new SendOtpDto { Email = "test@example.com", Username = "testuser", Password = "password123" };

        // Act
        var result = await controller.SendOtp(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task VerifyOtp_ReturnsOk_WithSuccessMessage()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        mockService.Setup(s => s.VerifyOtpAsync(It.IsAny<VerifyOtpDto>())).ReturnsAsync("Verification successful");

        var controller = new AuthController(mockService.Object);
        var dto = new VerifyOtpDto { Email = "test@example.com", Otp = "123456" };

        // Act
        var result = await controller.VerifyOtp(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
    }

    [Fact]
    public async Task Login_ReturnsOk_WithToken()
    {
        // Arrange
        var mockService = new Mock<IAuthService>();
        var tokenResponse = new TokenResponseDto 
        { 
            Token = "mocked-jwt-token", 
            Username = "testuser",
            Role = "User",
            ExpiresAt = System.DateTime.UtcNow.AddHours(24)
        };
        mockService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>())).ReturnsAsync(tokenResponse);

        var controller = new AuthController(mockService.Object);
        var dto = new LoginDto { Email = "test@example.com", Password = "password123" };

        // Act
        var result = await controller.Login(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returned = Assert.IsType<TokenResponseDto>(okResult.Value);
        Assert.Equal("mocked-jwt-token", returned.Token);
    }
}
