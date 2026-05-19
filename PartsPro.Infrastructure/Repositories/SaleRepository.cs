using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class SaleRepository : RepositoryBase<Sale>, ISaleRepository
{
    private readonly AppDbContext _context;

    public SaleRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Sale?> GetByIdWithItemsAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<List<Sale>> GetAllWithItemsAsync()
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Sale>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<Sale?> GetByIdWithItemsAndCustomerAsync(int id)
    {
        return await _context.Sales
            .Include(s => s.Customer)
            .Include(s => s.Items)
            .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public void Update(Sale sale)
    {
        _context.Sales.Update(sale);
    }
}