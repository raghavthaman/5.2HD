using System.ComponentModel.DataAnnotations;

namespace AboriginalArtGallery.API.DTOs
{
    public class CreateOrderDto
    {
        // ── Billing ──────────────────────────────────────────────────────────
        [Required(ErrorMessage = "Billing first name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "First name must be between 1 and 50 characters.")]
        public string BillingFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing last name is required.")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Last name must be between 1 and 50 characters.")]
        public string BillingLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing address line 1 is required.")]
        [StringLength(200, ErrorMessage = "Address line 1 must not exceed 200 characters.")]
        public string BillingAddressLine1 { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Address line 2 must not exceed 200 characters.")]
        public string? BillingAddressLine2 { get; set; }

        [Required(ErrorMessage = "Billing city is required.")]
        [StringLength(100, ErrorMessage = "City must not exceed 100 characters.")]
        public string BillingCity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing state is required.")]
        [StringLength(100, ErrorMessage = "State must not exceed 100 characters.")]
        public string BillingState { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing post code is required.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Post code must be exactly 4 digits (Australian format).")]
        public string BillingPostCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Billing country is required.")]
        [StringLength(100, ErrorMessage = "Country must not exceed 100 characters.")]
        public string BillingCountry { get; set; } = "Australia";

        [Required(ErrorMessage = "Billing phone number is required.")]
        [Phone(ErrorMessage = "A valid phone number is required.")]
        [StringLength(20, ErrorMessage = "Phone number must not exceed 20 characters.")]
        public string BillingPhone { get; set; } = string.Empty;

        // ── Delivery ─────────────────────────────────────────────────────────
        public bool SameAsDelivery { get; set; }

        [StringLength(50, ErrorMessage = "First name must not exceed 50 characters.")]
        public string? DeliveryFirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name must not exceed 50 characters.")]
        public string? DeliveryLastName { get; set; }

        [StringLength(200, ErrorMessage = "Address line 1 must not exceed 200 characters.")]
        public string? DeliveryAddressLine1 { get; set; }

        [StringLength(200, ErrorMessage = "Address line 2 must not exceed 200 characters.")]
        public string? DeliveryAddressLine2 { get; set; }

        [StringLength(100, ErrorMessage = "City must not exceed 100 characters.")]
        public string? DeliveryCity { get; set; }

        [StringLength(100, ErrorMessage = "State must not exceed 100 characters.")]
        public string? DeliveryState { get; set; }

        [RegularExpression(@"^(\d{4})?$", ErrorMessage = "Post code must be exactly 4 digits (Australian format).")]
        public string? DeliveryPostCode { get; set; }

        [StringLength(100, ErrorMessage = "Country must not exceed 100 characters.")]
        public string? DeliveryCountry { get; set; }

        // ── Delivery scheduling ───────────────────────────────────────────────
        [Required(ErrorMessage = "Preferred delivery date is required.")]
        public DateTime PreferredDeliveryDate { get; set; }

        [Required(ErrorMessage = "Delivery time slot is required.")]
        [RegularExpression(@"^(?i)(morning|afternoon|evening)$", ErrorMessage = "Time slot must be morning, afternoon, or evening.")]
        public string DeliveryTimeSlot { get; set; } = string.Empty;

        // ── Gift ─────────────────────────────────────────────────────────────
        public bool IsGift { get; set; }

        [StringLength(300, ErrorMessage = "Gift message must not exceed 300 characters.")]
        public string? GiftMessage { get; set; }

        [StringLength(50, ErrorMessage = "Gift wrap style must not exceed 50 characters.")]
        public string? GiftWrapStyle { get; set; }

        // ── Promo / Redeem ────────────────────────────────────────────────────
        [StringLength(30, ErrorMessage = "Coupon code must not exceed 30 characters.")]
        public string? CouponCode { get; set; }

        [StringLength(30, ErrorMessage = "Redeem code must not exceed 30 characters.")]
        public string? RedeemCode { get; set; }

        // ── Payment ───────────────────────────────────────────────────────────
        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression(@"^(card|paypal|bank_transfer)$", ErrorMessage = "Payment method must be 'card', 'paypal', or 'bank_transfer'.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [RegularExpression(@"^(\d{4})?$", ErrorMessage = "Card last four digits must be exactly 4 digits.")]
        public string? CardLastFour { get; set; }

        [StringLength(20, ErrorMessage = "Card brand must not exceed 20 characters.")]
        public string? CardBrand { get; set; }

        // ── Items ─────────────────────────────────────────────────────────────
        [Required(ErrorMessage = "Order must contain at least one item.")]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A valid artifact ID is required.")]
        public int ArtifactId { get; set; }

        [Range(1, 50, ErrorMessage = "Quantity must be between 1 and 50.")]
        public int Quantity { get; set; } = 1;
    }
}
