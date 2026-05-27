using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Middleware;
using AboriginalArtGallery.API.Repositories.Implementations;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Services.Implementations;
using AboriginalArtGallery.API.Services.Interfaces;

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
