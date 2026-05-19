using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.Parts;

public class CreatePartRequest
{
    [Required(ErrorMessage = "Part name is required")]
    [MaxLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Part number is required")]
    [MaxLength(50)]
    public string PartNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required")]
    [MaxLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selling price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Selling price must be greater than 0")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Cost price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Cost price must be greater than 0")]
    public decimal CostPrice { get; set; }

    [Required(ErrorMessage = "Stock is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Vendor id is required")]
    public int VendorId { get; set; }
}