namespace AboriginalArtGallery.API.DTOs;

public class OrderResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Username { get; set; }
    public DateTime CreatedAt { get; set; }

    public string BillingFirstName { get; set; } = string.Empty;
    public string BillingLastName { get; set; } = string.Empty;
    public string BillingAddressLine1 { get; set; } = string.Empty;
    public string? BillingAddressLine2 { get; set; }
    public string BillingCity { get; set; } = string.Empty;
    public string BillingState { get; set; } = string.Empty;
    public string BillingPostCode { get; set; } = string.Empty;
    public string BillingCountry { get; set; } = "Australia";
    public string BillingPhone { get; set; } = string.Empty;

    public bool SameAsDelivery { get; set; }
    public string DeliveryFirstName { get; set; } = string.Empty;
    public string DeliveryLastName { get; set; } = string.Empty;
    public string DeliveryAddressLine1 { get; set; } = string.Empty;
    public string? DeliveryAddressLine2 { get; set; }
    public string DeliveryCity { get; set; } = string.Empty;
    public string DeliveryState { get; set; } = string.Empty;
    public string DeliveryPostCode { get; set; } = string.Empty;
    public string DeliveryCountry { get; set; } = "Australia";

    public DateTime PreferredDeliveryDate { get; set; }
    public string DeliveryTimeSlot { get; set; } = string.Empty;

    public bool IsGift { get; set; }
    public string? GiftMessage { get; set; }
    public string? GiftWrapStyle { get; set; }

    public string? CouponCode { get; set; }
    public string? RedeemCode { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public string? CardLastFour { get; set; }
    public string? CardBrand { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? DiscountType { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal GiftWrapFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = "Pending";

    public List<OrderItemResponseDto> OrderItems { get; set; } = new();
}
