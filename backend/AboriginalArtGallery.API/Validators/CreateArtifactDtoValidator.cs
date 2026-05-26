using FluentValidation;
using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Validators;

public class CreateArtifactDtoValidator : AbstractValidator<CreateArtifactDto>
{
    public CreateArtifactDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0.");

        RuleFor(x => x.ArtistId)
            .GreaterThan(0).WithMessage("Valid Artist ID is required.");
    }
}
