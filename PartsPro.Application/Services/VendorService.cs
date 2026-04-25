using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs;
using PartsPro.Application.Interfaces;
using PartsPro.Domain.Entities;
 
namespace PartsPro.Application.Services;
 
public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepo;
    private readonly ILogger<VendorService> _logger;
 
    public VendorService(IVendorRepository vendorRepo, ILogger<VendorService> logger)
    {
        _vendorRepo = vendorRepo;
        _logger = logger;
    }
 
    public async Task<List<VendorResponse>> GetAllVendorsAsync()
    {
        var vendors = await _vendorRepo.GetAllAsync();
        return vendors.Select(MapToResponse).ToList();
    }
 
    public async Task<VendorResponse> GetVendorByIdAsync(int id)
    {
        var vendor = await _vendorRepo.GetByIdWithPartsAsync(id)
            ?? throw new KeyNotFoundException($"Vendor with ID {id} not found.");
        return MapToResponse(vendor);
    }
 
    public async Task<VendorResponse> CreateVendorAsync(VendorRequest request)
    {
        // Check duplicate email
        if (await _vendorRepo.ExistsByEmailAsync(request.Email))
            throw new InvalidOperationException($"A vendor with email '{request.Email}' already exists.");
 
        var vendor = new Vendor
        {
            Name          = request.Name.Trim(),
            ContactPerson = request.ContactPerson.Trim(),
            Phone         = request.Phone.Trim(),
            Email         = request.Email.Trim().ToLower(),
            Address       = request.Address.Trim(),
            CreatedAt     = DateTime.UtcNow
        };
 
        await _vendorRepo.AddAsync(vendor);
        _logger.LogInformation("Vendor created: {Name} (ID: {Id})", vendor.Name, vendor.Id);
        return MapToResponse(vendor);
    }
 
    public async Task<VendorResponse> UpdateVendorAsync(int id, VendorRequest request)
    {
        var vendor = await _vendorRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Vendor with ID {id} not found.");
 
        // Check duplicate email — exclude current vendor
        if (await _vendorRepo.ExistsByEmailAsync(request.Email, excludeId: id))
            throw new InvalidOperationException($"Email '{request.Email}' is already used by another vendor.");
 
        vendor.Name          = request.Name.Trim();
        vendor.ContactPerson = request.ContactPerson.Trim();
        vendor.Phone         = request.Phone.Trim();
        vendor.Email         = request.Email.Trim().ToLower();
        vendor.Address       = request.Address.Trim();
 
        await _vendorRepo.UpdateAsync(vendor);
        _logger.LogInformation("Vendor updated: {Name} (ID: {Id})", vendor.Name, vendor.Id);
        return MapToResponse(vendor);
    }
 
    public async Task DeleteVendorAsync(int id)
    {
        var vendor = await _vendorRepo.GetByIdWithPartsAsync(id)
            ?? throw new KeyNotFoundException($"Vendor with ID {id} not found.");
 
        // Can't delete vendor if parts are linked — would break FK constraint
        if (vendor.Parts.Any())
            throw new InvalidOperationException(
                $"Cannot delete vendor '{vendor.Name}' because {vendor.Parts.Count} part(s) are linked to it. Reassign or delete those parts first.");
 
        await _vendorRepo.DeleteAsync(vendor);
        _logger.LogInformation("Vendor deleted: {Name} (ID: {Id})", vendor.Name, vendor.Id);
    }
 
    private static VendorResponse MapToResponse(Vendor v) => new()
    {
        Id            = v.Id,
        Name          = v.Name,
        ContactPerson = v.ContactPerson,
        Phone         = v.Phone,
        Email         = v.Email,
        Address       = v.Address,
        CreatedAt     = v.CreatedAt,
        TotalParts    = v.Parts?.Count ?? 0
    };
}