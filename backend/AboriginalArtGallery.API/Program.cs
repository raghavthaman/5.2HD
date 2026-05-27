using AboriginalArtGallery.API.Data;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Register custom infrastructure services
builder.Services.AddGalleryInfrastructure(builder.Configuration);

var app = builder.Build();

// Setup the custom middleware pipeline
app.UseGalleryPipeline(app.Configuration);

app.MapControllers();

// Seed database on startup
DbInitializer.Seed(app);

app.Run();
