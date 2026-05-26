namespace AboriginalArtGallery.API.DTOs;

public class OrderItemResponseDto
{
    public int Id { get; set; }
    public int ArtifactId { get; set; }
    public string ArtifactTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
}
