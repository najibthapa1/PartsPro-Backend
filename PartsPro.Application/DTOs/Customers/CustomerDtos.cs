using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Customers;

public class CustomerProfileResponse
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public int VehicleCount { get; set; }
    public int SaleCount { get; set; }
    public decimal TotalSpent { get; set; }
    public int LoyaltyPoints { get; set; }
}

public class UpdateCustomerRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [Phone]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(250)]
    public string Address { get; set; } = string.Empty;
}

public class AddVehicleRequest
{
    [Required]
    [StringLength(20)]
    public string PlateNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [StringLength(250)]
    public string? Notes { get; set; }
}

public class CustomerVehicleResponse
{
    public int Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerSaleResponse
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerAppointmentResponse
{
    public int Id { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerPartRequestResponse
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CustomerReviewResponse
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CustomerCreditRecordResponse
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CustomerHistoryResponse
{
    public CustomerProfileResponse Profile { get; set; } = null!;
    public List<CustomerVehicleResponse> Vehicles { get; set; } = new();
    public List<CustomerSaleResponse> Sales { get; set; } = new();
    public List<CustomerAppointmentResponse> Appointments { get; set; } = new();
    public List<CustomerPartRequestResponse> PartRequests { get; set; } = new();
    public List<CustomerReviewResponse> Reviews { get; set; } = new();
    public List<CustomerCreditRecordResponse> CreditRecords { get; set; } = new();
    public decimal TotalSpent { get; set; }
    public int LoyaltyPoints { get; set; }
    public decimal CreditBalance { get; set; }
    public DateTime? LastActivityDate { get; set; }
}

