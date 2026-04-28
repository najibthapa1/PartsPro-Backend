using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface IPurchaseInvoiceRepository : IRepositoryBase<PurchaseInvoice>
{
    Task<PurchaseInvoice?> GetByIdWithItemsAsync(int id);

    Task<List<PurchaseInvoice>> GetAllWithItemsAsync();

    Task<List<PurchaseInvoice>> GetByVendorIdAsync(int vendorId);
}