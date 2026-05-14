using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

public class ReportRepository : RepositoryBase<Sale>, IReportRepository
{
    private readonly AppDbContext _context;

    public ReportRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all sales within a date range
    /// </summary>
    public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .Include(s => s.Items)
            .ThenInclude(si => si.Part)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all purchase invoices within a date range
    /// </summary>
    public async Task<List<PurchaseInvoice>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.PurchaseInvoices
            .Where(pi => pi.PurchasedAt >= startDate && pi.PurchasedAt <= endDate)
            .Include(pi => pi.Items)
            .ThenInclude(pui => pui.Part)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all parts with their inventory details
    /// </summary>
    public async Task<List<Part>> GetAllPartsWithInventoryAsync()
    {
        return await _context.Parts
            .Include(p => p.SaleItems)
            .Include(p => p.PurchaseItems)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get sales grouped by month
    /// </summary>
    public async Task<List<(int Month, int Year, decimal Revenue, int Count)>> GetMonthlySalesAsync()
    {
        var monthlySales = await _context.Sales
            .AsNoTracking()
            .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Month,
                g.Key.Year,
                Revenue = g.Sum(s => s.FinalAmount),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();

        return monthlySales
            .Select(x => (x.Month, x.Year, x.Revenue, x.Count))
            .ToList();
    }

    /// <summary>
    /// Get purchases grouped by month
    /// </summary>
    public async Task<List<(int Month, int Year, decimal Cost, int Count)>> GetMonthlyPurchasesAsync()
    {
        var monthlyPurchases = await _context.PurchaseInvoices
            .AsNoTracking()
            .GroupBy(pi => new { pi.PurchasedAt.Year, pi.PurchasedAt.Month })
            .Select(g => new
            {
                g.Key.Month,
                g.Key.Year,
                Cost = g.Sum(pi => pi.TotalCost),
                Count = g.Count()
            })
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .ToListAsync();

        return monthlyPurchases
            .Select(x => (x.Month, x.Year, x.Cost, x.Count))
            .ToList();
    }

    /// <summary>
    /// Get top selling parts
    /// </summary>
    public async Task<List<(int PartId, string PartName, string PartNumber, int QuantitySold, decimal Revenue)>> GetTopSellingPartsAsync(int limit = 10)
    {
        var topParts = await _context.SaleItems
            .Include(si => si.Part)
            .AsNoTracking()
            .GroupBy(si => new { si.PartId, si.Part.Name, si.Part.PartNumber })
            .Select(g => new
            {
                PartId = g.Key.PartId,
                PartName = g.Key.Name,
                PartNumberStr = g.Key.PartNumber,
                QuantitySold = g.Sum(si => si.Quantity),
                Revenue = g.Sum(si => si.LineTotal)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(limit)
            .ToListAsync();

        return topParts
            .Select(x => (x.PartId, x.PartName, x.PartNumberStr, x.QuantitySold, x.Revenue))
            .ToList();
    }

    /// <summary>
    /// Get daily sales data
    /// </summary>
    public async Task<List<(DateTime Date, decimal Revenue, int Count)>> GetDailySalesAsync(DateTime startDate, DateTime endDate)
    {
        var dailySales = await _context.Sales
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .AsNoTracking()
            .GroupBy(s => s.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Revenue = g.Sum(s => s.FinalAmount),
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return dailySales
            .Select(x => (x.Date, x.Revenue, x.Count))
            .ToList();
    }

    /// <summary>
    /// Get low stock parts (Stock less than 10)
    /// </summary>
    public async Task<List<Part>> GetLowStockPartsAsync()
    {
        return await _context.Parts
            .Where(p => p.Stock < 10)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get total number of active customers in date range (customers who made a sale)
    /// </summary>
    public async Task<int> GetActiveCustomersCountAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Sales
            .Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate)
            .AsNoTracking()
            .Select(s => s.CustomerId)
            .Distinct()
            .CountAsync();
    }

    /// <summary>
    /// Get total number of vendors
    /// </summary>
    public async Task<int> GetTotalVendorsCountAsync()
    {
        return await _context.Vendors
            .AsNoTracking()
            .CountAsync();
    }
    
    public async Task<int> GetTotalCustomersCountAsync()
        => await _context.Customers.AsNoTracking().CountAsync();
}

