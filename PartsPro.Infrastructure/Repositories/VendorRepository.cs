using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;
 
namespace PartsPro.Infrastructure.Repositories;
 
public class VendorRepository : IVendorRepository
{
    private readonly AppDbContext _context;
 
    public VendorRepository(AppDbContext context)
    {
        _context = context;
    }
 
    public async Task<List<Vendor>> GetAllAsync() =>
        await _context.Vendors
            .Include(v => v.Parts)
            .OrderBy(v => v.Name)
            .ToListAsync();
 
    public async Task<Vendor?> GetByIdAsync(int id) =>
        await _context.Vendors.FindAsync(id);
 
    public async Task<Vendor?> GetByIdWithPartsAsync(int id) =>
        await _context.Vendors
            .Include(v => v.Parts)
            .FirstOrDefaultAsync(v => v.Id == id);
 
    public async Task<bool> ExistsByEmailAsync(string email, int? excludeId = null) =>
        await _context.Vendors
            .AnyAsync(v => v.Email == email.ToLower() &&
                (excludeId == null || v.Id != excludeId));
 
    public async Task AddAsync(Vendor vendor)
    {
        await _context.Vendors.AddAsync(vendor);
        await _context.SaveChangesAsync();
    }
 
    public async Task UpdateAsync(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
        await _context.SaveChangesAsync();
    }
 
    public async Task DeleteAsync(Vendor vendor)
    {
        _context.Vendors.Remove(vendor);
        await _context.SaveChangesAsync();
    }
}