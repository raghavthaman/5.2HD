using System.ComponentModel.DataAnnotations;

namespace AboriginalArtGallery.API.DTOs;

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}