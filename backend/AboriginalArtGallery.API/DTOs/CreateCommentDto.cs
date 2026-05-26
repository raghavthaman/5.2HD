namespace AboriginalArtGallery.API.DTOs;

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public int ArtifactId { get; set; }
}
