using PartsPro.Domain.Enums;

namespace PartsPro.Domain.Entities;

public class Part
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    // Selling price used in sales invoices.
    public decimal Price { get; set; }

    public decimal CostPrice { get; set; }

    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign key
    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    public List<SaleItem> SaleItems { get; set; } = new();
    public List<PurchaseItem> PurchaseItems { get; set; } = new();
}