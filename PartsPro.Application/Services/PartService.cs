using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs;
using PartsPro.Application.Interfaces;
using PartsPro.Domain.Entities;
 
namespace PartsPro.Application.Services;
 
public class PartService : IPartService
{
    private readonly IPartRepository _partRepo;
    private readonly IVendorRepository _vendorRepo;
    private readonly ILogger<PartService> _logger;
 
    public PartService(
        IPartRepository partRepo,
        IVendorRepository vendorRepo,
        ILogger<PartService> logger)
    {
        _partRepo   = partRepo;
        _vendorRepo = vendorRepo;
        _logger     = logger;
    }
 
    public async Task<List<PartResponse>> GetAllPartsAsync()
    {
        var parts = await _partRepo.GetAllAsync();
        return parts.Select(MapToResponse).ToList();
    }
 
    public async Task<PartResponse> GetPartByIdAsync(int id)
    {
        var part = await _partRepo.GetByIdWithVendorAsync(id)
            ?? throw new KeyNotFoundException($"Part with ID {id} not found.");
        return MapToResponse(part);
    }
 
    public async Task<List<PartResponse>> GetLowStockPartsAsync()
    {
        var parts = await _partRepo.GetLowStockPartsAsync();
        return parts.Select(MapToResponse).ToList();
    }
 
    public async Task<PartResponse> CreatePartAsync(PartRequest request)
    {
        // Validate vendor exists
        var vendor = await _vendorRepo.GetByIdAsync(request.VendorId)
            ?? throw new KeyNotFoundException($"Vendor with ID {request.VendorId} not found.");
 
        // Check duplicate part number
        if (await _partRepo.ExistsByPartNumberAsync(request.PartNumber))
            throw new InvalidOperationException(
                $"Part number '{request.PartNumber}' already exists.");
 
        if (request.Price <= 0)
            throw new InvalidOperationException("Price must be greater than zero.");
 
        if (request.Stock < 0)
            throw new InvalidOperationException("Stock cannot be negative.");
 
        var part = new Part
        {
            Name       = request.Name.Trim(),
            PartNumber = request.PartNumber.Trim().ToUpper(),
            Category   = request.Category.Trim(),
            Price      = request.Price,
            Stock      = request.Stock,
            VendorId   = request.VendorId,
            CreatedAt  = DateTime.UtcNow
        };
 
        await _partRepo.AddAsync(part);
        _logger.LogInformation("Part created: {Name} ({PartNumber})", part.Name, part.PartNumber);
 
        part.Vendor = vendor; // attach for mapping
        return MapToResponse(part);
    }
 
    public async Task<PartResponse> UpdatePartAsync(int id, PartRequest request)
    {
        var part = await _partRepo.GetByIdWithVendorAsync(id)
            ?? throw new KeyNotFoundException($"Part with ID {id} not found.");
 
        // Validate vendor
        var vendor = await _vendorRepo.GetByIdAsync(request.VendorId)
            ?? throw new KeyNotFoundException($"Vendor with ID {request.VendorId} not found.");
 
        // Check duplicate part number — exclude current part
        if (await _partRepo.ExistsByPartNumberAsync(request.PartNumber, excludeId: id))
            throw new InvalidOperationException(
                $"Part number '{request.PartNumber}' is already used by another part.");
 
        if (request.Price <= 0)
            throw new InvalidOperationException("Price must be greater than zero.");
 
        if (request.Stock < 0)
            throw new InvalidOperationException("Stock cannot be negative.");
 
        part.Name       = request.Name.Trim();
        part.PartNumber = request.PartNumber.Trim().ToUpper();
        part.Category   = request.Category.Trim();
        part.Price      = request.Price;
        part.Stock      = request.Stock;
        part.VendorId   = request.VendorId;
        part.Vendor     = vendor;
 
        await _partRepo.UpdateAsync(part);
        _logger.LogInformation("Part updated: {Name} (ID: {Id})", part.Name, part.Id);
        return MapToResponse(part);
    }
 
    public async Task DeletePartAsync(int id)
    {
        var part = await _partRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Part with ID {id} not found.");
 
        await _partRepo.DeleteAsync(part);
        _logger.LogInformation("Part deleted: ID {Id}", id);
    }
 
    private static PartResponse MapToResponse(Part p) => new()
    {
        Id         = p.Id,
        Name       = p.Name,
        PartNumber = p.PartNumber,
        Category   = p.Category,
        Price      = p.Price,
        Stock      = p.Stock,
        VendorId   = p.VendorId,
        VendorName = p.Vendor?.Name ?? string.Empty,
        CreatedAt  = p.CreatedAt
    };
}