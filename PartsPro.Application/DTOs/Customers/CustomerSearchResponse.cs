namespace PartsPro.Application.DTOs.Customers;

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

public class CustomerVehicleDto
{
    public string PlateNumber { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
}
