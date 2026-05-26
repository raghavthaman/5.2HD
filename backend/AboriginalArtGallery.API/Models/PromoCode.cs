namespace AboriginalArtGallery.API.Models
{
    public class PromoCode
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;           // e.g. "SAVE20"
        public string Type { get; set; } = "percentage";           // "percentage" | "fixed"
        public decimal Value { get; set; }                         // 20 = 20% off OR $20 off
        public bool IsActive { get; set; } = true;
        public DateTime? ExpiresAt { get; set; }
        public int MaxUses { get; set; } = 100;
        public int UsedCount { get; set; } = 0;
    }
}
