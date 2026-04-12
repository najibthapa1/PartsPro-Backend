using PartsPro.Domain.Enums;

namespace PartsPro.Domain.Entities;

public class PartRequest
{
    public int Id { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PartRequestUrgency Urgency { get; set; } = PartRequestUrgency.Medium;
    public bool IsResolved { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
}