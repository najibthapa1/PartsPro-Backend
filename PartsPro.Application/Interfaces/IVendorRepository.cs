using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces;

public interface IVendorRepository
{
    Task<List<Vendor>> GetAllAsync();
    Task<Vendor?> GetByIdAsync(int id);
    Task<Vendor?> GetByIdWithPartsAsync(int id);
    Task<bool> ExistsByEmailAsync(string email, int? excludeId = null);
    Task AddAsync(Vendor vendor);
    Task UpdateAsync(Vendor vendor);
    Task DeleteAsync(Vendor vendor);
}

