using FluentValidation;
using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Validators;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        RuleFor(x => x.BillingFirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BillingLastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BillingAddressLine1).NotEmpty().MaximumLength(150);
        RuleFor(x => x.BillingCity).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BillingState).NotEmpty().MaximumLength(50);
        RuleFor(x => x.BillingPostCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.BillingPhone).NotEmpty().MaximumLength(20);

        RuleFor(x => x.DeliveryFirstName).NotEmpty().MaximumLength(50).When(x => !x.SameAsDelivery);
        RuleFor(x => x.DeliveryLastName).NotEmpty().MaximumLength(50).When(x => !x.SameAsDelivery);
        RuleFor(x => x.DeliveryAddressLine1).NotEmpty().MaximumLength(150).When(x => !x.SameAsDelivery);
        RuleFor(x => x.DeliveryCity).NotEmpty().MaximumLength(50).When(x => !x.SameAsDelivery);
        RuleFor(x => x.DeliveryState).NotEmpty().MaximumLength(50).When(x => !x.SameAsDelivery);
        RuleFor(x => x.DeliveryPostCode).NotEmpty().MaximumLength(10).When(x => !x.SameAsDelivery);

        RuleFor(x => x.PreferredDeliveryDate).NotEmpty().Must(d => d >= DateTime.Today.AddDays(-1)).WithMessage("Preferred delivery date must be in the future.");
        RuleFor(x => x.DeliveryTimeSlot).NotEmpty().Must(slot => slot == "morning" || slot == "afternoon").WithMessage("Delivery time slot must be either 'morning' or 'afternoon'.");

        RuleFor(x => x.PaymentMethod).NotEmpty().Must(p => p == "card" || p == "paypal").WithMessage("Payment method must be 'card' or 'paypal'.");
        RuleFor(x => x.Items).NotEmpty().WithMessage("At least one artifact must be in the order.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ArtifactId).GreaterThan(0);
            item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
        });
    }
}
