using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

public interface IReportRepository : IRepositoryBase<Sale>
{
    /// <summary>
    /// Get all sales within a date range
    /// </summary>
    Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get all purchase invoices within a date range
    /// </summary>
    Task<List<PurchaseInvoice>> GetPurchasesByDateRangeAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get all parts with their inventory details
    /// </summary>
    Task<List<Part>> GetAllPartsWithInventoryAsync();

    /// <summary>
    /// Get sales grouped by month
    /// </summary>
    Task<List<(int Month, int Year, decimal Revenue, int Count)>> GetMonthlySalesAsync();

    /// <summary>
    /// Get purchases grouped by month
    /// </summary>
    Task<List<(int Month, int Year, decimal Cost, int Count, decimal PaidAmount, decimal UnpaidAmount)>> GetMonthlyPurchasesAsync();

    /// <summary>
    /// Get top selling parts
    /// </summary>
    Task<List<(int PartId, string PartName, string PartNumber, int QuantitySold, decimal Revenue)>> GetTopSellingPartsAsync(int limit = 10);

    /// <summary>
    /// Get daily sales data
    /// </summary>
    Task<List<(DateTime Date, decimal Revenue, int Count)>> GetDailySalesAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get low stock parts
    /// </summary>
    Task<List<Part>> GetLowStockPartsAsync();

    /// <summary>
    /// Get total number of active customers in date range
    /// </summary>
    Task<int> GetActiveCustomersCountAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get total number of vendors
    /// </summary>
    Task<int> GetTotalVendorsCountAsync();
    
    Task<int> GetTotalCustomersCountAsync();

}

