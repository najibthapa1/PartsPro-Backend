namespace PartsPro.Domain.Entities;

public class PurchaseItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal LineTotal => Quantity * UnitCost;

    public int PurchaseInvoiceId { get; set; }
    public PurchaseInvoice PurchaseInvoice { get; set; } = null!;

    public int PartId { get; set; }
    public Part Part { get; set; } = null!;
}