namespace PartsPro.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Address { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    
    public List<Vehicle> Vehicles { get; set; } = new();
    public List<Sale> Sales { get; set; } = new();
    public List<CreditRecord> CreditRecords { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
    public List<PartRequest> PartRequests { get; set; } = new();
    public List<Review> Reviews { get; set; } = new();
}