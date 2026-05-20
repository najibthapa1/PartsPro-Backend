using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Parts;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;

namespace PartsPro.Application.Services;

public class PartService : IPartService
{
    private readonly IPartRepository _partRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly ILogger<PartService> _logger;

    public PartService(
        IPartRepository partRepository,
        IVendorRepository vendorRepository,
        ILogger<PartService> logger)
    {
        _partRepository = partRepository;
        _vendorRepository = vendorRepository;
        _logger = logger;
    }

    public async Task<PartResponse> CreatePartAsync(CreatePartRequest request)
    {
        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId);
        if (vendor == null)
        {
            _logger.LogWarning("Create Part failed: Vendor {VendorId} not found", request.VendorId);
            throw new NotFoundException($"Vendor with ID {request.VendorId} not found");
        }

        var part = new Part
        {
            Name = request.Name,
            PartNumber = request.PartNumber,
            Category = request.Category,
            Price = request.Price,
            CostPrice = request.CostPrice,
            Stock = request.Stock,
            VendorId = request.VendorId
        };

        _partRepository.Create(part);
        await _partRepository.SaveChangesAsync();

        _logger.LogInformation("Part created successfully: {PartName} (ID: {PartId})", part.Name, part.Id);

        // Attach vendor data for the response if available.
        part.Vendor = vendor;
        return MapToResponse(part);
    }

    public async Task<PartResponse> GetPartByIdAsync(int id)
    {
        var part = await _partRepository.FindAll(trackChanges: false)
            .Include(p => p.Vendor)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (part == null)
        {
            _logger.LogWarning("Part with ID {Id} not found", id);
            throw new NotFoundException($"Part with ID {id} not found");
        }

        return MapToResponse(part);
    }

    public async Task<IEnumerable<PartResponse>> GetAllPartsAsync(int pageNumber = 1, int pageSize = 10)
    {
        var parts = await _partRepository.FindAll(trackChanges: false)
            .Include(p => p.Vendor)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return parts.Select(MapToResponse);
    }

    public async Task UpdatePartAsync(int id, CreatePartRequest request)
    {
        _logger.LogInformation("Updating part ID: {PartId}", id);

        var part = await _partRepository.GetByIdAsync(id);
        if (part == null)
        {
            _logger.LogWarning("Update failed: Part {PartId} not found", id);
            throw new NotFoundException($"Part with ID {id} not found");
        }

        if (part.VendorId != request.VendorId)
        {
            var vendor = await _vendorRepository.GetByIdAsync(request.VendorId);
            if (vendor == null)
            {
                throw new NotFoundException($"Vendor with ID {request.VendorId} not found");
            }
        }

        part.Name = request.Name;
        part.PartNumber = request.PartNumber;
        part.Category = request.Category;
        part.Price = request.Price;
        part.CostPrice = request.CostPrice;
        part.Stock = request.Stock;
        part.VendorId = request.VendorId;

        _partRepository.Update(part);
        await _partRepository.SaveChangesAsync();

        _logger.LogInformation("Part updated successfully: {PartName} (ID: {PartId})", part.Name, part.Id);
    }

    public async Task DeletePartAsync(int id)
    {
        _logger.LogInformation("Deleting part ID: {PartId}", id);

        var part = await _partRepository.GetByIdAsync(id);
        if (part == null)
        {
            _logger.LogWarning("Delete failed: Part {PartId} not found", id);
            throw new NotFoundException($"Part with ID {id} not found");
        }

        _partRepository.Delete(part);
        await _partRepository.SaveChangesAsync();

        _logger.LogInformation("Part deleted successfully (ID: {PartId})", id);
    }

    public async Task<IEnumerable<PartResponse>> SearchPartsAsync(string query)
    {
        var parts = await _partRepository.FindAll(trackChanges: false)
            .Include(p => p.Vendor)
            .Where(p => p.Name.Contains(query) || p.PartNumber.Contains(query))
            .Take(20)
            .ToListAsync();

        return parts.Select(MapToResponse);
    }

    private static PartResponse MapToResponse(Part part)
    {
        return new PartResponse
        {
            Id = part.Id,
            Name = part.Name,
            PartNumber = part.PartNumber,
            Category = part.Category,
            Price = part.Price,
            CostPrice = part.CostPrice,
            Stock = part.Stock,
            VendorId = part.VendorId,
            VendorName = part.Vendor?.Name ?? string.Empty,
            CreatedAt = part.CreatedAt
        };
    }
}
