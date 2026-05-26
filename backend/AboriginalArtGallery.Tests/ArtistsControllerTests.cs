using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AboriginalArtGallery.API.Controllers;
using AboriginalArtGallery.API.Models;
using AboriginalArtGallery.API.DTOs;
using AboriginalArtGallery.API.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AboriginalArtGallery.Tests;

public class ArtistsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsOk_WithArtistList()
    {
        // Arrange
        var mockService = new Mock<IArtistService>();
        var artistsList = new List<ArtistResponseDto>
        {
            new() { Id = 1, Name = "Emily Kame Kngwarreye", Tribe = "Anmatyerre" },
            new() { Id = 2, Name = "Albert Namatjira", Tribe = "Arrernte" }
        };
        mockService.Setup(s => s.GetAllArtistsAsync()).ReturnsAsync(artistsList);

        var controller = new ArtistsController(mockService.Object);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedArtists = Assert.IsAssignableFrom<IEnumerable<ArtistResponseDto>>(okResult.Value);
        Assert.Equal(2, returnedArtists.Count());
    }

    [Fact]
    public async Task GetById_ReturnsOk_WithArtist()
    {
        // Arrange
        var mockService = new Mock<IArtistService>();
        var artistDto = new ArtistResponseDto { Id = 1, Name = "Emily Kame Kngwarreye", Tribe = "Anmatyerre" };
        
        mockService.Setup(s => s.GetArtistByIdAsync(1, It.IsAny<HttpRequest>())).ReturnsAsync(artistDto);

        var controller = new ArtistsController(mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        // Act
        var result = await controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedArtist = Assert.IsType<ArtistResponseDto>(okResult.Value);
        Assert.Equal(1, returnedArtist.Id);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNewArtist()
    {
        // Arrange
        var mockService = new Mock<IArtistService>();
        var artist = new Artist { Id = 3, Name = "Rover Thomas", Tribe = "Kukatja" };
        mockService.Setup(s => s.CreateArtistAsync(It.IsAny<Artist>())).ReturnsAsync(artist);

        var controller = new ArtistsController(mockService.Object);

        // Act
        var result = await controller.Create(artist);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<Artist>(createdResult.Value);
        Assert.Equal("Rover Thomas", returned.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenArtistDeleted()
    {
        // Arrange
        var mockService = new Mock<IArtistService>();
        mockService.Setup(s => s.DeleteArtistAsync(1)).Returns(Task.CompletedTask);

        var controller = new ArtistsController(mockService.Object);

        // Act
        var result = await controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}