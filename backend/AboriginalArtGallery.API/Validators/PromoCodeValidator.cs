using FluentValidation;
using AboriginalArtGallery.API.Models;

namespace AboriginalArtGallery.API.Validators;

public class PromoCodeValidator : AbstractValidator<PromoCode>
{
    public PromoCodeValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Promo code is required.")
            .MaximumLength(50).WithMessage("Promo code must not exceed 50 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Type is required.")
            .Must(t => t == "percentage" || t == "fixed").WithMessage("Type must be 'percentage' or 'fixed'.");

        RuleFor(x => x.Value)
            .GreaterThan(0).WithMessage("Value must be greater than 0.");

        RuleFor(x => x.MaxUses)
            .GreaterThan(0).WithMessage("Max uses must be greater than 0.");
    }
}
