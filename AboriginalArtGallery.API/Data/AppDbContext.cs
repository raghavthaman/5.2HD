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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ArtifactTag>()
            .HasKey(at => new { at.ArtifactId, at.TagId });

        modelBuilder.Entity<Artist>()
            .HasIndex(a => a.Name)
            .IsUnique();
    }
}