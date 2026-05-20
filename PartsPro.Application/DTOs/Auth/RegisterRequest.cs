using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;

    // Vehicle details
    [Required(ErrorMessage = "Vehicle plate number is required")]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle model is required")]
    [MaxLength(100)]
    public string VehicleModel { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? VehicleMake { get; set; }

    public int VehicleYear { get; set; }
}


