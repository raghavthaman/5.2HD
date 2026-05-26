using FluentValidation;
using AboriginalArtGallery.API.DTOs;

namespace AboriginalArtGallery.API.Validators;

public class PromoteUserDtoValidator : AbstractValidator<PromoteUserDto>
{
    public PromoteUserDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => r == "Admin" || r == "User").WithMessage("Role must be either 'Admin' or 'User'.");
    }
}
