using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.PurchaseInvoices;

public class CreatePurchaseInvoiceRequest
{
    [Required]
    public int VendorId { get; set; }

    [Required]
    [MinLength(1, ErrorMessage = "At least one purchase item is required.")]
    public List<CreatePurchaseItemRequest> Items { get; set; } = new();

    public string Status { get; set; } = "Paid";
}