namespace PartsPro.Application.DTOs.Parts;

public class PartResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
