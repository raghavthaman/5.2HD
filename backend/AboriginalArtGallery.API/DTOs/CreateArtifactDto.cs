namespace AboriginalArtGallery.API.DTOs;

public class CreateArtifactDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ArtType { get; set; }
    public int? YearCreated { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsAvailableForPurchase { get; set; } = false;
    public int StockQuantity { get; set; } = 1;
    public int ArtistId { get; set; }
}
