namespace AboriginalArtGallery.API.DTOs;

public class ArtifactResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ArtType { get; set; }
    public int? YearCreated { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailableForPurchase { get; set; }
    public int StockQuantity { get; set; }
    public int ArtistId { get; set; }
    public string? ArtistName { get; set; }
    public List<string> Tags { get; set; } = new();
    public List<CommentResponseDto> Comments { get; set; } = new();
}
