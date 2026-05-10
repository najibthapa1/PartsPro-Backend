using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartsPro.DTOs.CustomerDTOs
{
    public class ProfileDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalPurchases { get; set; }
        public int TotalServices { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateProfileDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 100 characters")]
        public string FullName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 digits")]
        public string PhoneNumber { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string Address { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password is required")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
        public string ConfirmNewPassword { get; set; }
    }

    public class VehicleDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vehicle number is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Vehicle number must be between 5 and 50 characters")]
        public string VehicleNumber { get; set; }

        [Required(ErrorMessage = "Make is required")]
        [StringLength(100, ErrorMessage = "Make cannot exceed 100 characters")]
        public string Make { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2026, ErrorMessage = "Year must be between 1900 and 2026")]
        public int Year { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot exceed 50 characters")]
        public string Color { get; set; }

        [StringLength(50, ErrorMessage = "Engine number cannot exceed 50 characters")]
        public string EngineNumber { get; set; }

        [StringLength(50, ErrorMessage = "Chassis number cannot exceed 50 characters")]
        public string ChassisNumber { get; set; }
    }
}