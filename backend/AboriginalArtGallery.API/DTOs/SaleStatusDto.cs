namespace AboriginalArtGallery.API.DTOs;

public class SaleStatusDto
{
    public bool IsAvailableForPurchase { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
