namespace PartsPro.Domain.Entities;

public class Vendor
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ContactPerson { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Part> Parts { get; set; } = new();
    public List<PurchaseInvoice> PurchaseInvoices { get; set; } = new();
}