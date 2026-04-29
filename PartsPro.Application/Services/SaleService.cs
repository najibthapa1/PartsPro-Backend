using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Sales;
using PartsPro.Application.Exceptions;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;

namespace PartsPro.Application.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IPartRepository _partRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<SaleService> _logger;

    public SaleService(
        ISaleRepository saleRepository,
        IPartRepository partRepository,
        ICustomerRepository customerRepository,
        ILogger<SaleService> logger)
    {
        _saleRepository = saleRepository;
        _partRepository = partRepository;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task<SaleResponse> CreateSaleAsync(CreateSaleRequest request)
    {
        if (request.Items == null || !request.Items.Any())
        {
            _logger.LogWarning("Sale creation failed. No sale items were provided.");
            throw new BadRequestException("At least one sale item is required.");
        }

        var customer = await _customerRepository.GetByIdAsync(request.CustomerId);

        if (customer == null)
        {
            _logger.LogWarning("Sale creation failed. Customer ID {CustomerId} was not found.", request.CustomerId);
            throw new NotFoundException($"Customer with ID {request.CustomerId} not found.");
        }

        var saleItems = new List<SaleItem>();
        decimal totalAmount = 0;

        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                _logger.LogWarning(
                    "Sale creation failed. Invalid quantity {Quantity} for Part ID {PartId}.",
                    item.Quantity,
                    item.PartId);

                throw new BadRequestException("Quantity must be greater than zero.");
            }

            var part = await _partRepository.GetByIdAsync(item.PartId);

            if (part == null)
            {
                _logger.LogWarning("Sale creation failed. Part ID {PartId} was not found.", item.PartId);
                throw new NotFoundException($"Part with ID {item.PartId} not found.");
            }

            if (part.Stock < item.Quantity)
            {
                _logger.LogWarning(
                    "Sale creation failed. Insufficient stock for Part ID {PartId}. Available: {AvailableStock}, Requested: {RequestedQuantity}",
                    part.Id,
                    part.Stock,
                    item.Quantity);

                throw new BadRequestException(
                    $"Insufficient stock for part '{part.Name}'. Available stock: {part.Stock}.");
            }

            var lineTotal = part.Price * item.Quantity;
            totalAmount += lineTotal;

            part.Stock -= item.Quantity;

            saleItems.Add(new SaleItem
            {
                PartId = part.Id,
                Quantity = item.Quantity,
                UnitPrice = part.Price
            });
        }

        var loyaltyDiscountApplied = totalAmount > 5000;
        var discountAmount = loyaltyDiscountApplied ? totalAmount * 0.10m : 0;
        var finalAmount = totalAmount - discountAmount;

        var sale = new Sale
        {
            CustomerId = request.CustomerId,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            LoyaltyDiscountApplied = loyaltyDiscountApplied,
            IsEmailSent = false,
            CreatedAt = DateTime.UtcNow,
            Items = saleItems
        };

        _saleRepository.Create(sale);
        await _saleRepository.SaveChangesAsync();

        _logger.LogInformation(
            "Sale created successfully. Sale ID: {SaleId}, Customer ID: {CustomerId}, Final Amount: {FinalAmount}",
            sale.Id,
            sale.CustomerId,
            sale.FinalAmount);

        var createdSale = await _saleRepository.GetByIdWithItemsAsync(sale.Id);

        if (createdSale == null)
        {
            throw new NotFoundException($"Sale with ID {sale.Id} not found after creation.");
        }

        return MapToResponse(createdSale);
    }

    public async Task<SaleResponse> GetSaleByIdAsync(int id)
    {
        var sale = await _saleRepository.GetByIdWithItemsAsync(id);

        if (sale == null)
        {
            _logger.LogWarning("Sale lookup failed. Sale ID {SaleId} was not found.", id);
            throw new NotFoundException($"Sale with ID {id} not found.");
        }

        return MapToResponse(sale);
    }

    public async Task<List<SaleResponse>> GetAllSalesAsync()
    {
        var sales = await _saleRepository.GetAllWithItemsAsync();

        return sales
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<SaleResponse>> GetSalesByCustomerIdAsync(int customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);

        if (customer == null)
        {
            _logger.LogWarning("Sales lookup failed. Customer ID {CustomerId} was not found.", customerId);
            throw new NotFoundException($"Customer with ID {customerId} not found.");
        }

        var sales = await _saleRepository.GetByCustomerIdAsync(customerId);

        return sales
            .Select(MapToResponse)
            .ToList();
    }

    private static SaleResponse MapToResponse(Sale sale)
    {
        return new SaleResponse
        {
            Id = sale.Id,
            CustomerId = sale.CustomerId,
            CustomerName = sale.Customer?.FullName ?? string.Empty,
            TotalAmount = sale.TotalAmount,
            DiscountAmount = sale.DiscountAmount,
            FinalAmount = sale.FinalAmount,
            LoyaltyDiscountApplied = sale.LoyaltyDiscountApplied,
            CreatedAt = sale.CreatedAt,
            Items = sale.Items.Select(item => new SaleItemResponse
            {
                PartId = item.PartId,
                PartName = item.Part?.Name ?? string.Empty,
                PartNumber = item.Part?.PartNumber ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                LineTotal = item.UnitPrice * item.Quantity
            }).ToList()
        };
    }
}