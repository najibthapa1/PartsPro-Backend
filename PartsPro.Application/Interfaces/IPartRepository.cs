using PartsPro.Domain.Entities;
 
namespace PartsPro.Application.Interfaces;
 
public interface IPartRepository
{
    Task<List<Part>> GetAllAsync();
    Task<Part?> GetByIdAsync(int id);
    Task<Part?> GetByIdWithVendorAsync(int id);
    Task<List<Part>> GetLowStockPartsAsync();         // F15 — stock < 10
    Task<bool> ExistsByPartNumberAsync(string partNumber, int? excludeId = null);
    Task AddAsync(Part part);
    Task UpdateAsync(Part part);
    Task DeleteAsync(Part part);
}
public class PartResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsLowStock => Stock < 10;   // used by F15
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}