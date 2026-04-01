// Tests/AuthControllerTests.cs
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using AboriginalArtGallery.API.Controllers;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.DTOs;
 
namespace AboriginalArtGallery.Tests;
 
public class AuthControllerTests
{
    private AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName).Options;
        return new AppDbContext(options);
    }
 
    private IConfiguration CreateConfig()
    {
        var inMemorySettings = new Dictionary<string, string> {
            ["JwtSettings:Key"] = "TestSuperSecretKeyThatIsLongEnough!123",
            ["JwtSettings:Issuer"] = "TestIssuer",
            ["JwtSettings:Audience"] = "TestAudience",
            ["JwtSettings:ExpiryHours"] = "24"
        };
        return new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();
    }
 
    [Fact]
    public async Task Register_ReturnsCreated_WhenValidInput()
    {
        using var context = CreateContext("RegisterTest");
        var controller = new AuthController(context, CreateConfig());
 
        var dto = new RegisterDto {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
 
        var result = await controller.Register(dto);
 
        Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(1, await context.Users.CountAsync());
    }
 
    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenWrongPassword()
    {
        using var context = CreateContext("LoginWrongPass");
        var controller = new AuthController(context, CreateConfig());
 
        // Register first
        await controller.Register(new RegisterDto {
            Username = "user2", Email = "user2@test.com", Password = "rightPassword"
        });
 
        // Try login with wrong password
        var result = await controller.Login(new LoginDto {
            Email = "user2@test.com", Password = "wrongPassword"
        });
 
        Assert.IsType<UnauthorizedObjectResult>(result);
    }
}
