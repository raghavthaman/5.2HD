namespace AboriginalArtGallery.API.DTOs;

public class ArtistResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Tribe { get; set; }
    public string? Biography { get; set; }
    public int? BirthYear { get; set; }
    public string Country { get; set; } = "Australia";
    public DateTime CreatedAt { get; set; }
    public List<ArtifactResponseDto> Artifacts { get; set; } = new();
}
