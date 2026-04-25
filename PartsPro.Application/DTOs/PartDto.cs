namespace PartsPro.Application.DTOs;
 
public class PartRequest
{
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int VendorId { get; set; }
}