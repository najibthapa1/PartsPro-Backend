using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Vendors;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;

namespace PartsPro.Application.Services;

/// <summary>
/// Service for managing vendor data and operations
/// </summary>
public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly ILogger<VendorService> _logger;

    public VendorService(IVendorRepository vendorRepository, ILogger<VendorService> logger)
    {
        _vendorRepository = vendorRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieve a single vendor by ID
    /// </summary>
    public async Task<VendorResponse?> GetVendorByIdAsync(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            _logger.LogWarning($"Vendor with ID {id} not found");
            throw new NotFoundException($"Vendor with ID {id} not found");
        }

        return MapToResponse(vendor);
    }

    /// <summary>
    /// Get paginated list of all vendors
    /// </summary>
    public async Task<IEnumerable<VendorResponse>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var vendors = await _vendorRepository.FindAll(trackChanges: false)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return vendors.Select(MapToResponse);
    }

    /// <summary>
    /// Create a new vendor record
    /// </summary>
    public async Task<VendorResponse> CreateVendorAsync(VendorRequest request)
    {
        var vendor = new Vendor
        {
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };

        _vendorRepository.Create(vendor);
        await _vendorRepository.SaveChangesAsync();

        _logger.LogInformation($"Vendor created: {vendor.Name} (ID: {vendor.Id})");

        return MapToResponse(vendor);
    }

    /// <summary>
    /// Update existing vendor details
    /// </summary>
    public async Task UpdateVendorAsync(int id, VendorRequest request)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            _logger.LogWarning($"Update failed: Vendor {id} not found");
            throw new NotFoundException($"Vendor with ID {id} not found");
        }

        vendor.Name = request.Name;
        vendor.ContactPerson = request.ContactPerson;
        vendor.Email = request.Email;
        vendor.Phone = request.Phone;
        vendor.Address = request.Address;

        _vendorRepository.Update(vendor);
        await _vendorRepository.SaveChangesAsync();

        _logger.LogInformation($"Vendor updated: {vendor.Name} (ID: {vendor.Id})");
    }

    /// <summary>
    /// Delete a vendor from the system
    /// </summary>
    public async Task DeleteVendorAsync(int id)
    {
        var vendor = await _vendorRepository.GetByIdAsync(id);
        if (vendor == null)
        {
            _logger.LogWarning($"Delete failed: Vendor {id} not found");
            throw new NotFoundException($"Vendor with ID {id} not found");
        }

        _vendorRepository.Delete(vendor);
        await _vendorRepository.SaveChangesAsync();

        _logger.LogInformation($"Vendor deleted: {vendor.Name} (ID: {id})");
    }

    private static VendorResponse MapToResponse(Vendor vendor)
    {
        return new VendorResponse
        {
            Id = vendor.Id,
            Name = vendor.Name,
            ContactPerson = vendor.ContactPerson,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Address = vendor.Address,
            CreatedAt = vendor.CreatedAt
        };
    }
}
