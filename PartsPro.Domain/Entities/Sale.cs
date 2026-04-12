namespace PartsPro.Domain.Entities;

public class Sale
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }      
    public decimal DiscountAmount { get; set; }   
    public decimal FinalAmount { get; set; }      
    public bool LoyaltyDiscountApplied { get; set; } = false;
    public bool IsEmailSent { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public List<SaleItem> Items { get; set; } = new();
}