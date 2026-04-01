namespace AboriginalArtGallery.API.Models;

public class ArtifactTag
{
    public int ArtifactId { get; set; }
    public Artifact Artifact { get; set; } = null!;

    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}