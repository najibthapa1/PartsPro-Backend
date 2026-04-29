using PartsPro.Application.DTOs.Reports;

namespace PartsPro.Application.Interfaces.Services;

public interface IReportService
{
    /// <summary>
    /// Get comprehensive financial report for a date range
    /// </summary>
    Task<FinancialReportResponse> GetFinancialReportAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get monthly sales summary for the current year
    /// </summary>
    Task<IEnumerable<MonthlySalesResponse>> GetMonthlySalesReportAsync(int year);

    /// <summary>
    /// Get monthly purchase summary for the current year
    /// </summary>
    Task<IEnumerable<MonthlyPurchaseResponse>> GetMonthlyPurchaseReportAsync(int year);

    /// <summary>
    /// Get top selling parts report
    /// </summary>
    Task<IEnumerable<TopSellingPartResponse>> GetTopSellingPartsReportAsync(int limit = 10);

    /// <summary>
    /// Get inventory summary report
    /// </summary>
    Task<InventorySummaryResponse> GetInventorySummaryAsync();

    /// <summary>
    /// Get daily sales report for a date range
    /// </summary>
    Task<IEnumerable<DailySalesResponse>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate);

    /// <summary>
    /// Get profit and loss statement
    /// </summary>
    Task<FinancialReportResponse> GetProfitAndLossStatementAsync(DateTime startDate, DateTime endDate);
}

