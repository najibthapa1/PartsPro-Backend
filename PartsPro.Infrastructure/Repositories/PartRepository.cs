using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;
 
namespace PartsPro.Infrastructure.Repositories;
 
public class PartRepository : IPartRepository
{
    private readonly AppDbContext _context;
 
    public PartRepository(AppDbContext context)
    {
        _context = context;
    }
 
    public async Task<List<Part>> GetAllAsync() =>
        await _context.Parts
            .Include(p => p.Vendor)
            .OrderBy(p => p.Name)
            .ToListAsync();
 
    public async Task<Part?> GetByIdAsync(int id) =>
        await _context.Parts.FindAsync(id);
 
    public async Task<Part?> GetByIdWithVendorAsync(int id) =>
        await _context.Parts
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == id);
 
    // F15 — used by background job for low stock alerts
    public async Task<List<Part>> GetLowStockPartsAsync() =>
        await _context.Parts
            .Include(p => p.Vendor)
            .Where(p => p.Stock < 10)
            .OrderBy(p => p.Stock)
            .ToListAsync();
 
    public async Task<bool> ExistsByPartNumberAsync(string partNumber, int? excludeId = null) =>
        await _context.Parts
            .AnyAsync(p => p.PartNumber == partNumber.ToUpper() &&
                          (excludeId == null || p.Id != excludeId));
 
    public async Task AddAsync(Part part)
    {
        await _context.Parts.AddAsync(part);
        await _context.SaveChangesAsync();
    }
 
    public async Task UpdateAsync(Part part)
    {
        _context.Parts.Update(part);
        await _context.SaveChangesAsync();
    }
 
    public async Task DeleteAsync(Part part)
    {
        _context.Parts.Remove(part);
        await _context.SaveChangesAsync();
    }
}