using System.ComponentModel.DataAnnotations;

namespace AboriginalArtGallery.API.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "A valid email address is required.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Password is required.")]
    public string Password { get; set; } = string.Empty;
}