namespace PartsPro.Domain.Entities;

public class SaleItem
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal => Quantity * UnitPrice;

    public int SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    public int PartId { get; set; }
    public Part Part { get; set; } = null!;
}