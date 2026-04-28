namespace PartsPro.Application.DTOs.PurchaseInvoices;

public class PurchaseInvoiceResponse
{
    public int Id { get; set; }

    public int VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public decimal TotalCost { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime PurchasedAt { get; set; }

    public List<PurchaseItemResponse> Items { get; set; } = new();
}