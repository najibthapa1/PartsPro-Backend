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
    public bool LoyaltyDiscountApplied { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<CustomerSaleItemResponse> Items { get; set; } = new();
}

public class CustomerSaleItemResponse
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }
}

public class CreateAppointmentRequest
{
    [Required]
    public int VehicleId { get; set; }

    [Required]
    [StringLength(150)]
    public string ServiceType { get; set; } = string.Empty;

    [Required]
    public DateTime AppointmentDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}

public class UpdateAppointmentStatusRequest
{
    [Required]
    public string Status { get; set; } = string.Empty;
}

public class CustomerAppointmentResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int VehicleId { get; set; }
    public string VehiclePlateNumber { get; set; } = string.Empty;
    public string VehicleModel { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePartRequestCustomerRequest
{
    [Required]
    [StringLength(150)]
    public string PartName { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Urgency { get; set; } = "Medium";
}

public class UpdatePartRequestStatusRequest
{
    public bool IsResolved { get; set; }
}

public class CustomerPartRequestResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Urgency { get; set; } = string.Empty;
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateReviewRequest
{
    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [Required]
    [StringLength(700, MinimumLength = 3)]
    public string Comment { get; set; } = string.Empty;
}

public class CustomerReviewResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
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

public class CustomerInsightSummaryResponse
{
    public int RegularCustomers { get; set; }
    public int HighSpenders { get; set; }
    public int CustomersWithPendingCredits { get; set; }
    public decimal TotalPendingCredit { get; set; }
    public List<CustomerInsightCustomerResponse> RegularCustomerList { get; set; } = new();
    public List<CustomerInsightCustomerResponse> HighSpenderList { get; set; } = new();
    public List<CustomerInsightCreditResponse> PendingCreditList { get; set; } = new();
}

public class CustomerInsightCustomerResponse
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int SaleCount { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
}

public class CustomerInsightCreditResponse
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public decimal TotalCreditOwed { get; set; }
    public DateTime? OldestCreditDate { get; set; }
}

public class CustomerVehicleDto
{
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}

public class CustomerSearchResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public decimal TotalCreditOwed { get; set; }
    public List<CustomerVehicleDto> Vehicles { get; set; } = new();
}
