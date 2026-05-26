using FluentValidation;
using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Validators;

public class CreateCommentDtoValidator : AbstractValidator<CreateCommentDto>
{
    public CreateCommentDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Comment content cannot be empty.")
            .MaximumLength(1000).WithMessage("Comment content must not exceed 1000 characters.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("Valid User ID is required.");

        RuleFor(x => x.ArtifactId)
            .GreaterThan(0).WithMessage("Valid Artifact ID is required.");
    }
}
