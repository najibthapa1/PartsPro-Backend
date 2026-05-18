using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Auth;

// This DTO is used when a staff member registers a customer from the counter
// Unlike self-registration, vehicle info is mandatory here and password is auto-generated
public class StaffCustomerRegisterRequest
{
    [Required(ErrorMessage = "Full name is required")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    public string Address { get; set; } = string.Empty;

    // Vehicle details are mandatory when staff registers the customer
    [Required(ErrorMessage = "Vehicle plate number is required")]
    [MaxLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vehicle model is required")]
    [MaxLength(100)]
    public string VehicleModel { get; set; } = string.Empty;

    // Make is optional since some customers might not know it
    [MaxLength(100)]
    public string? VehicleMake { get; set; }

    [Required(ErrorMessage = "Vehicle year is required")]
    public int VehicleYear { get; set; }
}
