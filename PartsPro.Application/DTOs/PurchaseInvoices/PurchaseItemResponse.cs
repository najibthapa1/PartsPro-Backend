namespace PartsPro.Application.DTOs.PurchaseInvoices;

public class PurchaseItemResponse
{
    public int PartId { get; set; }

    public string PartName { get; set; } = string.Empty;

    public string PartNumber { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal LineTotal { get; set; }
}