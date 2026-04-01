using System.ComponentModel.DataAnnotations;

namespace AboriginalArtGallery.API.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;

    public ICollection<ArtifactTag> ArtifactTags { get; set; } = new List<ArtifactTag>();
}