using System.Text.Json.Serialization;

namespace AboriginalArtGallery.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        // Addresses
        public string BillingFirstName { get; set; } = string.Empty;
        public string BillingLastName { get; set; } = string.Empty;
        public string BillingAddressLine1 { get; set; } = string.Empty;
        public string? BillingAddressLine2 { get; set; }
        public string BillingCity { get; set; } = string.Empty;
        public string BillingState { get; set; } = string.Empty;
        public string BillingPostCode { get; set; } = string.Empty;
        public string BillingCountry { get; set; } = "Australia";
        public string BillingPhone { get; set; } = string.Empty;

        public bool SameAsDelivery { get; set; } = false;
        public string? DeliveryFirstName { get; set; }
        public string? DeliveryLastName { get; set; }
        public string? DeliveryAddressLine1 { get; set; }
        public string? DeliveryAddressLine2 { get; set; }
        public string? DeliveryCity { get; set; }
        public string? DeliveryState { get; set; }
        public string? DeliveryPostCode { get; set; }
        public string? DeliveryCountry { get; set; }

        // Delivery scheduling
        public DateTime PreferredDeliveryDate { get; set; }
        public string DeliveryTimeSlot { get; set; } = string.Empty; // e.g. "09:00–12:00"

        // Gift wrapping
        public bool IsGift { get; set; } = false;
        public string? GiftMessage { get; set; }
        public string? GiftWrapStyle { get; set; } // "standard", "premium", "luxury"

        // Discounts / Promo
        public string? CouponCode { get; set; }
        public string? RedeemCode { get; set; }     // loyalty / gift card redeem
        public decimal DiscountAmount { get; set; } = 0;
        public string? DiscountType { get; set; }   // "percentage" | "fixed"

        // Payment
        public string PaymentMethod { get; set; } = string.Empty; // "card" | "paypal" | "afterpay"
        public string? CardLastFour { get; set; }
        public string? CardBrand { get; set; }       // "visa" | "mastercard" | "amex"

        // Totals
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal GiftWrapFee { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }

        // Status
        public string Status { get; set; } = "Pending"; // Pending | Confirmed | Shipped | Delivered | Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
