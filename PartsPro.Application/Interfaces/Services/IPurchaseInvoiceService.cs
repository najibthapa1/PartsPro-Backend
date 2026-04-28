using PartsPro.Application.DTOs.PurchaseInvoices;

namespace PartsPro.Application.Interfaces.Services;

public interface IPurchaseInvoiceService
{
    Task<PurchaseInvoiceResponse> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request);

    Task<PurchaseInvoiceResponse> GetPurchaseInvoiceByIdAsync(int id);

    Task<List<PurchaseInvoiceResponse>> GetAllPurchaseInvoicesAsync();

    Task<List<PurchaseInvoiceResponse>> GetPurchaseInvoicesByVendorIdAsync(int vendorId);
}