using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class PurchaseInvoiceRepository : RepositoryBase<PurchaseInvoice>, IPurchaseInvoiceRepository
{
    private readonly AppDbContext _context;

    public PurchaseInvoiceRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<PurchaseInvoice?> GetByIdWithItemsAsync(int id)
    {
        return await _context.PurchaseInvoices
            .Include(p => p.Vendor)
            .Include(p => p.Items)
                .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<PurchaseInvoice>> GetAllWithItemsAsync()
    {
        return await _context.PurchaseInvoices
            .Include(p => p.Vendor)
            .Include(p => p.Items)
                .ThenInclude(i => i.Part)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync();
    }

    public async Task<List<PurchaseInvoice>> GetByVendorIdAsync(int vendorId)
    {
        return await _context.PurchaseInvoices
            .Include(p => p.Vendor)
            .Include(p => p.Items)
                .ThenInclude(i => i.Part)
            .Where(p => p.VendorId == vendorId)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync();
    }
}