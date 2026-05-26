using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AboriginalArtGallery.API.Models;

public class Artifact
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? ArtType { get; set; }
    public int? YearCreated { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsAvailableForPurchase { get; set; } = true;
    public int StockQuantity { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ArtistId { get; set; }
    [JsonIgnore]
    public Artist? Artist { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ArtifactTag> ArtifactTags { get; set; } = new List<ArtifactTag>();
}