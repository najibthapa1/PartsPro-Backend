namespace PartsPro.Application.DTOs.Sales;

public class SaleResponse
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    public bool LoyaltyDiscountApplied { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<SaleItemResponse> Items { get; set; } = new();
}