using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AboriginalArtGallery.API.Models;

public class Comment
{
    public int Id { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }
    public int ArtifactId { get; set; }

[JsonIgnore]
public User? User { get; set; }

[JsonIgnore]
public Artifact? Artifact { get; set; }

}