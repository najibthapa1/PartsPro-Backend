using Microsoft.Extensions.Logging;
using PartsPro.Application.DTOs.Reports;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Enums;

namespace PartsPro.Application.Services;

/// <summary>
/// Service for generating financial and inventory reports
/// </summary>
public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly ILogger<ReportService> _logger;

    public ReportService(IReportRepository reportRepository, ILogger<ReportService> logger)
    {
        _reportRepository = reportRepository;
        _logger = logger;
    }

    /// <summary>
    /// Get comprehensive financial report for a date range
    /// </summary>
    public async Task<FinancialReportResponse> GetFinancialReportAsync(DateTime startDate, DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        _logger.LogInformation($"Generating financial report from {startDate} to {endDate}");

        // Get sales data
        var sales = await _reportRepository.GetSalesByDateRangeAsync(startDate, endDate);
        var totalSalesRevenue = sales.Sum(s => s.FinalAmount);
        var totalSalesCount = sales.Count;
        var totalSalesDiscounts = sales.Sum(s => s.DiscountAmount);
        var averageSaleAmount = totalSalesCount > 0 ? totalSalesRevenue / totalSalesCount : 0;

        // Get purchase data
        var purchases = await _reportRepository.GetPurchasesByDateRangeAsync(startDate, endDate);
        var totalPurchaseCost = purchases.Sum(pi => pi.TotalCost);
        var totalPurchasesCount = purchases.Count;
        var totalPurchasesPaid = purchases
            .Where(pi => pi.Status == InvoiceStatus.Paid)
            .Sum(pi => pi.TotalCost);
        var totalPurchasesUnpaid = purchases
            .Where(pi => pi.Status == InvoiceStatus.Unpaid)
            .Sum(pi => pi.TotalCost);
        var averagePurchaseAmount = totalPurchasesCount > 0 ? totalPurchaseCost / totalPurchasesCount : 0;

        // Get inventory data
        var parts = await _reportRepository.GetAllPartsWithInventoryAsync();
        var totalInventoryValue = parts.Sum(p => p.Price * p.Stock);
        var lowStockParts = parts.Where(p => p.Stock < 10).ToList();

        // Calculate financial metrics
        var costOfGoodsSold = purchases.Sum(pi => pi.Items.Sum(pui => pui.LineTotal));
        var grossProfit = totalSalesRevenue - costOfGoodsSold;
        var grossProfitMargin = totalSalesRevenue > 0 ? (grossProfit / totalSalesRevenue) * 100 : 0;
        var netProfit = grossProfit - totalPurchasesUnpaid;

        // Get customer metrics
        var activeCustomers = await _reportRepository.GetActiveCustomersCountAsync(startDate, endDate);
        var totalCustomersCount = await _reportRepository.GetTotalCustomersCountAsync();
        var averageOrderValue = totalSalesCount > 0 ? totalSalesRevenue / totalSalesCount : 0;

        // Get vendor metrics
        var totalVendors = await _reportRepository.GetTotalVendorsCountAsync();

        var report = new FinancialReportResponse
        {
            ReportDate = DateTime.UtcNow,
            StartDate = startDate,
            EndDate = endDate,
            TotalSalesRevenue = totalSalesRevenue,
            TotalSalesCount = totalSalesCount,
            AverageSaleAmount = averageSaleAmount,
            TotalSalesDiscounts = totalSalesDiscounts,
            TotalPurchaseCost = totalPurchaseCost,
            TotalPurchasesCount = totalPurchasesCount,
            AveragePurchaseAmount = averagePurchaseAmount,
            TotalPurchasesPaid = totalPurchasesPaid,
            TotalPurchasesUnpaid = totalPurchasesUnpaid,
            TotalPartsInInventory = parts.Count,
            TotalInventoryValue = totalInventoryValue,
            LowStockPartsCount = lowStockParts.Count,
            GrossProfit = grossProfit,
            NetProfit = netProfit,
            CostOfGoodsSold = costOfGoodsSold,
            GrossProfitMargin = grossProfitMargin,
            TotalCustomers = totalCustomersCount,
            ActiveCustomers = activeCustomers,
            AverageOrderValue = averageOrderValue,
            TotalVendors = totalVendors,
            ActiveVendors = purchases.Select(pi => pi.VendorId).Distinct().Count()
        };

        _logger.LogInformation($"Financial report generated successfully");
        return report;
    }

    /// <summary>
    /// Get financial report for a full year
    /// </summary>
    public async Task<FinancialReportResponse> GetYearlyFinancialReportAsync(int year)
    {
        var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1).AddTicks(-1);

        return await GetFinancialReportAsync(startDate, endDate);
    }

    /// <summary>
    /// Get monthly sales summary for the current year
    /// </summary>
    public async Task<IEnumerable<MonthlySalesResponse>> GetMonthlySalesReportAsync(int year)
    {
        _logger.LogInformation($"Generating monthly sales report for year {year}");

        var monthlySales = await _reportRepository.GetMonthlySalesAsync();
        var filteredSales = monthlySales
            .Where(x => x.Year == year)
            .OrderBy(x => x.Month)
            .Select(x => new MonthlySalesResponse
            {
                Month = x.Month,
                Year = x.Year,
                TotalRevenue = x.Revenue,
                SalesCount = x.Count,
                AverageOrderValue = x.Count > 0 ? x.Revenue / x.Count : 0
            })
            .ToList();

        // Fill in missing months with zero values
        var allMonths = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var existing = filteredSales.FirstOrDefault(x => x.Month == month);
                return existing ?? new MonthlySalesResponse
                {
                    Month = month,
                    Year = year,
                    TotalRevenue = 0,
                    SalesCount = 0,
                    AverageOrderValue = 0
                };
            })
            .ToList();

        return allMonths;
    }

    /// <summary>
    /// Get monthly purchase summary for the current year
    /// </summary>
    public async Task<IEnumerable<MonthlyPurchaseResponse>> GetMonthlyPurchaseReportAsync(int year)
    {
        _logger.LogInformation($"Generating monthly purchase report for year {year}");

        var monthlyPurchases = await _reportRepository.GetMonthlyPurchasesAsync();
        var filteredPurchases = monthlyPurchases
            .Where(x => x.Year == year)
            .OrderBy(x => x.Month)
            .Select(x => new MonthlyPurchaseResponse
            {
                Month = x.Month,
                Year = x.Year,
                TotalCost = x.Cost,
                PurchaseCount = x.Count,
                PaidAmount = x.PaidAmount,
                UnpaidAmount = x.UnpaidAmount
            })
            .ToList();

        // Fill in missing months with zero values
        var allMonths = Enumerable.Range(1, 12)
            .Select(month =>
            {
                var existing = filteredPurchases.FirstOrDefault(x => x.Month == month);
                return existing ?? new MonthlyPurchaseResponse
                {
                    Month = month,
                    Year = year,
                    TotalCost = 0,
                    PurchaseCount = 0,
                    PaidAmount = 0,
                    UnpaidAmount = 0
                };
            })
            .ToList();

        return allMonths;
    }

    /// <summary>
    /// Get top selling parts report
    /// </summary>
    public async Task<IEnumerable<TopSellingPartResponse>> GetTopSellingPartsReportAsync(int limit = 10)
    {
        _logger.LogInformation($"Generating top selling parts report (limit: {limit})");

        var topParts = await _reportRepository.GetTopSellingPartsAsync(limit);

        return topParts.Select(x => new TopSellingPartResponse
        {
            PartId = x.PartId,
            PartName = x.PartName,
            PartNumber = x.PartNumber,
            TotalQuantitySold = x.QuantitySold,
            TotalRevenue = x.Revenue
        }).ToList();
    }

    /// <summary>
    /// Get inventory summary report
    /// </summary>
    public async Task<InventorySummaryResponse> GetInventorySummaryAsync()
    {
        _logger.LogInformation("Generating inventory summary report");

        var parts = await _reportRepository.GetAllPartsWithInventoryAsync();
        var lowStockParts = await _reportRepository.GetLowStockPartsAsync();

        var inventorySummary = new InventorySummaryResponse
        {
            TotalPartsInStock = parts.Count,
            TotalInventoryValue = parts.Sum(p => p.Price * p.Stock),
            LowStockCount = lowStockParts.Count,
            OutOfStockCount = parts.Count(p => p.Stock == 0),
            LowStockParts = lowStockParts.Select(p => new LowStockPartResponse
            {
                PartId = p.Id,
                PartName = p.Name,
                CurrentStock = p.Stock,
                ReorderLevel = 10
            }).ToList()
        };

        return inventorySummary;
    }

    /// <summary>
    /// Get daily sales report for a date range
    /// </summary>
    public async Task<IEnumerable<DailySalesResponse>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        _logger.LogInformation($"Generating daily sales report from {startDate} to {endDate}");

        var dailySales = await _reportRepository.GetDailySalesAsync(startDate, endDate);

        return dailySales.Select(x => new DailySalesResponse
        {
            Date = x.Date,
            TotalRevenue = x.Revenue,
            SalesCount = x.Count
        }).ToList();
    }

    /// <summary>
    /// Get profit and loss statement
    /// </summary>
    public async Task<FinancialReportResponse> GetProfitAndLossStatementAsync(DateTime startDate, DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        _logger.LogInformation($"Generating profit and loss statement from {startDate} to {endDate}");

        // This uses the same logic as GetFinancialReportAsync
        return await GetFinancialReportAsync(startDate, endDate);
    }

    private static DateTime EnsureUtc(DateTime value)
        => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}

