using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Staff;

public class UpdateStaffRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string? Phone { get; set; }

    [Required]
    public string Department { get; set; } = string.Empty;
}
