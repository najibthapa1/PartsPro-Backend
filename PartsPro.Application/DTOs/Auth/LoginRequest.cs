using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = string.Empty;
}
