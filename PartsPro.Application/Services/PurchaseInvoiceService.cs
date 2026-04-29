using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.PurchaseInvoices;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;
using PartsPro.Domain.Enums;

namespace PartsPro.Application.Services;

public class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly IPurchaseInvoiceRepository _purchaseInvoiceRepository;
    private readonly IVendorRepository _vendorRepository;
    private readonly IPartRepository _partRepository;
    private readonly ILogger<PurchaseInvoiceService> _logger;

    public PurchaseInvoiceService(
        IPurchaseInvoiceRepository purchaseInvoiceRepository,
        IVendorRepository vendorRepository,
        IPartRepository partRepository,
        ILogger<PurchaseInvoiceService> logger)
    {
        _purchaseInvoiceRepository = purchaseInvoiceRepository;
        _vendorRepository = vendorRepository;
        _partRepository = partRepository;
        _logger = logger;
    }

    public async Task<PurchaseInvoiceResponse> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceRequest request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            _logger.LogWarning("Purchase invoice creation failed. No purchase items were provided.");
            throw new InvalidOperationException("At least one purchase item is required.");
        }

        var vendor = await _vendorRepository.GetByIdAsync(request.VendorId);

        if (vendor == null)
        {
            _logger.LogWarning("Purchase invoice creation failed. Vendor ID {VendorId} was not found.", request.VendorId);
            throw new NotFoundException($"Vendor with ID {request.VendorId} not found.");
        }

        var purchaseItems = new List<PurchaseItem>();
        decimal totalCost = 0;

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                _logger.LogWarning(
                    "Purchase invoice creation failed. Invalid quantity {Quantity} for Part ID {PartId}.",
                    item.Quantity,
                    item.PartId);

                throw new InvalidOperationException("Quantity must be greater than zero.");
            }

            if (item.UnitCost <= 0)
            {
                _logger.LogWarning(
                    "Purchase invoice creation failed. Invalid unit cost {UnitCost} for Part ID {PartId}.",
                    item.UnitCost,
                    item.PartId);

                throw new InvalidOperationException("Unit cost must be greater than zero.");
            }

            var part = await _partRepository.GetByIdAsync(item.PartId);

            if (part == null)
            {
                _logger.LogWarning("Purchase invoice creation failed. Part ID {PartId} was not found.", item.PartId);
                throw new NotFoundException($"Part with ID {item.PartId} not found.");
            }

            var lineTotal = item.UnitCost * item.Quantity;
            totalCost += lineTotal;

            part.Stock += item.Quantity;

            purchaseItems.Add(new PurchaseItem
            {
                PartId = part.Id,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost
            });
        }

        var status = InvoiceStatus.Paid;

        if (!string.IsNullOrWhiteSpace(request.Status) &&
            Enum.TryParse<InvoiceStatus>(request.Status, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var purchaseInvoice = new PurchaseInvoice
        {
            VendorId = request.VendorId,
            TotalCost = totalCost,
            Status = status,
            PurchasedAt = DateTime.UtcNow,
            Items = purchaseItems
        };

        _purchaseInvoiceRepository.Create(purchaseInvoice);
        await _purchaseInvoiceRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Purchase invoice created successfully. PurchaseInvoice ID: {PurchaseInvoiceId}, Vendor ID: {VendorId}, Total Cost: {TotalCost}",
            purchaseInvoice.Id,
            purchaseInvoice.VendorId,
            purchaseInvoice.TotalCost);

        var createdPurchaseInvoice = await _purchaseInvoiceRepository.GetByIdWithItemsAsync(purchaseInvoice.Id);

        if (createdPurchaseInvoice == null)
        {
            throw new NotFoundException($"Purchase invoice with ID {purchaseInvoice.Id} not found after creation.");
        }

        return MapToResponse(createdPurchaseInvoice);
    }

    public async Task<PurchaseInvoiceResponse> GetPurchaseInvoiceByIdAsync(int id)
    {
        var purchaseInvoice = await _purchaseInvoiceRepository.GetByIdWithItemsAsync(id);

        if (purchaseInvoice == null)
        {
            _logger.LogWarning("Purchase invoice lookup failed. PurchaseInvoice ID {PurchaseInvoiceId} was not found.", id);
            throw new NotFoundException($"Purchase invoice with ID {id} not found.");
        }

        return MapToResponse(purchaseInvoice);
    }

    public async Task<List<PurchaseInvoiceResponse>> GetAllPurchaseInvoicesAsync()
    {
        var purchaseInvoices = await _purchaseInvoiceRepository.GetAllWithItemsAsync();

        return purchaseInvoices
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<PurchaseInvoiceResponse>> GetPurchaseInvoicesByVendorIdAsync(int vendorId)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);

        if (vendor == null)
        {
            _logger.LogWarning("Purchase invoice lookup failed. Vendor ID {VendorId} was not found.", vendorId);
            throw new NotFoundException($"Vendor with ID {vendorId} not found.");
        }

        var purchaseInvoices = await _purchaseInvoiceRepository.GetByVendorIdAsync(vendorId);

        return purchaseInvoices
            .Select(MapToResponse)
            .ToList();
    }

    private static PurchaseInvoiceResponse MapToResponse(PurchaseInvoice purchaseInvoice)
    {
        return new PurchaseInvoiceResponse
        {
            Id = purchaseInvoice.Id,
            VendorId = purchaseInvoice.VendorId,
            VendorName = purchaseInvoice.Vendor?.Name ?? string.Empty,
            TotalCost = purchaseInvoice.TotalCost,
            Status = purchaseInvoice.Status.ToString(),
            PurchasedAt = purchaseInvoice.PurchasedAt,
            Items = purchaseInvoice.Items.Select(item => new PurchaseItemResponse
            {
                PartId = item.PartId,
                PartName = item.Part?.Name ?? string.Empty,
                PartNumber = item.Part?.PartNumber ?? string.Empty,
                Quantity = item.Quantity,
                UnitCost = item.UnitCost,
                LineTotal = item.UnitCost * item.Quantity
            }).ToList()
        };
    }
}