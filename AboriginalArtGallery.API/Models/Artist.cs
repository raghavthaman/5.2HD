using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AboriginalArtGallery.API.Models;

public class Artist
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Tribe { get; set; }
    public string? Biography { get; set; }
    public int? BirthYear { get; set; }

    public string Country { get; set; } = "Australia";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]

    public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
}