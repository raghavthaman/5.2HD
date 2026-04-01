using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Controllers;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.Tests;

public class ArtistsControllerTests
{
    // This helper creates a fresh, empty database for EACH test
    // so tests never interfere with each other
    private AppDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
        return new AppDbContext(options);
    }

    [Fact]  // ← This marks it as a test
    public async Task GetAll_ReturnsOk_WithArtistList()
    {
        // ARRANGE — set up data
        using var context = CreateContext("GetAll_Test");
        context.Artists.AddRange(
            new Artist { Name = "Emily Kame Kngwarreye", Tribe = "Anmatyerre" },
            new Artist { Name = "Albert Namatjira", Tribe = "Arrernte" }
        );
        await context.SaveChangesAsync();
        var controller = new ArtistsController(context);

        // ACT — call the method
        var result = await controller.GetAll();

        // ASSERT — check the result
        var ok = Assert.IsType<OkObjectResult>(result);
        var artists = Assert.IsAssignableFrom<IEnumerable<Artist>>(ok.Value);
        Assert.Equal(2, artists.Count());
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenArtistDoesNotExist()
    {
        using var context = CreateContext("GetById_NotFound");
        var controller = new ArtistsController(context);

        var result = await controller.GetById(999); // ID that doesn't exist

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WithNewArtist()
    {
        using var context = CreateContext("Create_Test");
        var controller = new ArtistsController(context);
        var artist = new Artist { Name = "Rover Thomas", Tribe = "Kukatja" };

        var result = await controller.Create(artist);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        var returned = Assert.IsType<Artist>(created.Value);
        Assert.Equal("Rover Thomas", returned.Name);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenArtistDeleted()
    {
        using var context = CreateContext("Delete_Test");
        var artist = new Artist { Name = "Dorothy Napangardi" };
        context.Artists.Add(artist);
        await context.SaveChangesAsync();
        var controller = new ArtistsController(context);

        var result = await controller.Delete(artist.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(0, await context.Artists.CountAsync());
    }
}