using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Vendors;

public class VendorRequest
{
    [Required(ErrorMessage = "Vendor name is required")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "Vendor name must be between 5 and 30 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Contact person is required")]
    public string ContactPerson { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;
}
