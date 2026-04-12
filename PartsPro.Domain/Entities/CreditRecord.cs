using PartsPro.Domain.Enums;

namespace PartsPro.Domain.Entities;

public class CreditRecord
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PaidAt { get; set; }

    // Computed — used by F15 background job
    public bool IsOverdue =>
        Status == InvoiceStatus.Unpaid &&
        DateTime.UtcNow > CreatedAt.AddDays(30);

    // Foreign key
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}