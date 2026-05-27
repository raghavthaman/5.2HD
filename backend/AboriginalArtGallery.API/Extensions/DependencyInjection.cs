using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi;
using AboriginalArtGallery.API.Data;
using AboriginalArtGallery.API.Middleware;
using AboriginalArtGallery.API.Repositories.Interfaces;
using AboriginalArtGallery.API.Repositories.Implementations;
using AboriginalArtGallery.API.Services.Interfaces;
using AboriginalArtGallery.API.Services.Implementations;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddGalleryInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Database Configuration
        services.AddDbContext<AppDbContext>(options => 
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // 2. Authentication & Authorization
        var key = configuration["JwtSettings:Key"] ?? throw new InvalidOperationException("JWT settings key is missing.");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>  
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        // 3. AutoMapper and FluentValidation
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<AboriginalArtGallery.API.Data.AppDbContext>();

        // 4. HTTP Context Accessor
        services.AddHttpContextAccessor();

        // 5. Repositories
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IArtifactRepository, ArtifactRepository>();
        services.AddScoped<IArtistRepository, ArtistRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IOtpCodeRepository, OtpCodeRepository>();

        // 6. Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IArtifactService, ArtifactService>();
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IUserService, UserService>();

        // 7. Global Rate Limiter
        services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        QueueLimit = 0,
                        Window = TimeSpan.FromMinutes(1)
                    }));
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
        });

        // 8. HSTS Configuration
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });

        // 9. Controllers with JSON options
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        // 10. Swagger with JWT Bearer support
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "Aboriginal Art Gallery API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Bearer", document),
                    new List<string>()
                }
            });
        });

        // 11. CORS Configuration (Restricted in Prod, flexible in Dev)
        services.AddCors(options =>
        {
            options.AddPolicy("FrontendPolicy", policy =>
            {
                policy.SetIsOriginAllowed(origin =>
                {
                    if (origin == "null") return true;
                    if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri)) return false;
                    return uri.Host == "localhost" || uri.Host == "127.0.0.1";
                })
                .AllowAnyHeader()
                .AllowAnyMethod();
            });
        });

        return services;
    }

    public static IApplicationBuilder UseGalleryPipeline(this IApplicationBuilder app, IConfiguration configuration)
    {
        // Centralized exception handler
        app.UseMiddleware<GlobalExceptionMiddleware>();

        // Toggle Swagger dynamically via configuration
        var enableSwagger = configuration.GetValue<bool>("AppSettings:EnableSwagger");
        if (enableSwagger)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Toggle HSTS dynamically via configuration
        var enableHsts = configuration.GetValue<bool>("AppSettings:EnableHsts");
        if (enableHsts)
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        // Serve default documents (like index.html) for root request
        app.UseDefaultFiles();

        // Serve local artwork static files from wwwroot
        app.UseStaticFiles();

        app.UseRouting();

        app.UseCors("FrontendPolicy");

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization(); 

        return app;
    }
}
