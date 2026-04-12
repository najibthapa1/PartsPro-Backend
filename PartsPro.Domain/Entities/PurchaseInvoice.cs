using PartsPro.Domain.Enums;

namespace PartsPro.Domain.Entities;

public class PurchaseInvoice
{
    public int Id { get; set; }
    public decimal TotalCost { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    public int VendorId { get; set; }
    public Vendor Vendor { get; set; } = null!;

    public List<PurchaseItem> Items { get; set; } = new();
}