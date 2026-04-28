using System.ComponentModel.DataAnnotations;

namespace PartsPro.Application.DTOs.PurchaseInvoices;

public class CreatePurchaseItemRequest
{
    [Required]
    public int PartId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero.")]
    public int Quantity { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Unit cost must be greater than zero.")]
    public decimal UnitCost { get; set; }
}