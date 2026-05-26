using Microsoft.EntityFrameworkCore;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Artist> Artists { get; set; }
    public DbSet<Artifact> Artifacts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ArtifactTag> ArtifactTags { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<PromoCode> PromoCodes { get; set; }
    public DbSet<OtpCode> OtpCodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArtifactTag>()
            .HasKey(at => new { at.ArtifactId, at.TagId });

        modelBuilder.Entity<Artist>()
            .HasIndex(a => a.Name)
            .IsUnique();

        modelBuilder.Entity<PromoCode>()
            .HasIndex(p => p.Code)
            .IsUnique();

        // Seed data for PromoCodes
        modelBuilder.Entity<PromoCode>().HasData(
            new PromoCode { Id = 1, Code = "WELCOME20", Type = "percentage", Value = 20, IsActive = true, MaxUses = 100, UsedCount = 0 },
            new PromoCode { Id = 2, Code = "MINUS50", Type = "fixed", Value = 50, IsActive = true, MaxUses = 100, UsedCount = 0 }
        );
    }
}