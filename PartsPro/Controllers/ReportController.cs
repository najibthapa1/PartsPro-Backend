using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Reports;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportController(IReportService reportService) : ControllerBase
{
    private readonly IReportService _reportService = reportService;

    /// <summary>
    /// Get comprehensive financial report for a date range
    /// </summary>
    /// <param name="startDate">Report start date (yyyy-MM-dd)</param>
    /// <param name="endDate">Report end date (yyyy-MM-dd)</param>
    [HttpGet("financial")]
    public async Task<ActionResult<FinancialReportResponse>> GetFinancialReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        if (startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }

        var report = await _reportService.GetFinancialReportAsync(startDate, endDate);
        return Ok(report);
    }

    /// <summary>
    /// Get profit and loss statement for a date range
    /// </summary>
    /// <param name="startDate">Report start date (yyyy-MM-dd)</param>
    /// <param name="endDate">Report end date (yyyy-MM-dd)</param>
    [HttpGet("profit-loss")]
    public async Task<ActionResult<FinancialReportResponse>> GetProfitAndLoss(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        if (startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }

        var report = await _reportService.GetProfitAndLossStatementAsync(startDate, endDate);
        return Ok(report);
    }

    /// <summary>
    /// Get yearly financial summary
    /// </summary>
    /// <param name="year">Year for the report</param>
    [HttpGet("financial/yearly")]
    public async Task<ActionResult<FinancialReportResponse>> GetYearlyFinancialReport(
        [FromQuery] int year = 0)
    {
        if (year == 0)
        {
            year = DateTime.UtcNow.Year;
        }

        if (year < 2000 || year > DateTime.UtcNow.Year)
        {
            return BadRequest("Invalid year provided");
        }

        var report = await _reportService.GetYearlyFinancialReportAsync(year);
        return Ok(report);
    }

    /// <summary>
    /// Get monthly sales summary for a specific year
    /// </summary>
    /// <param name="year">Year for the report</param>
    [HttpGet("sales/monthly")]
    public async Task<ActionResult<IEnumerable<MonthlySalesResponse>>> GetMonthlySalesReport(
        [FromQuery] int year = 0)
    {
        if (year == 0)
        {
            year = DateTime.UtcNow.Year;
        }

        if (year < 2000 || year > DateTime.UtcNow.Year)
        {
            return BadRequest("Invalid year provided");
        }

        var report = await _reportService.GetMonthlySalesReportAsync(year);
        return Ok(report);
    }

    /// <summary>
    /// Get daily sales report for a date range
    /// </summary>
    /// <param name="startDate">Report start date (yyyy-MM-dd)</param>
    /// <param name="endDate">Report end date (yyyy-MM-dd)</param>
    [HttpGet("sales/daily")]
    public async Task<ActionResult<IEnumerable<DailySalesResponse>>> GetDailySalesReport(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);

        if (startDate > endDate)
        {
            return BadRequest("Start date must be before end date");
        }

        var report = await _reportService.GetDailySalesReportAsync(startDate, endDate);
        return Ok(report);
    }

    /// <summary>
    /// Get monthly purchase summary for a specific year
    /// </summary>
    /// <param name="year">Year for the report</param>
    [HttpGet("purchases/monthly")]
    public async Task<ActionResult<IEnumerable<MonthlyPurchaseResponse>>> GetMonthlyPurchaseReport(
        [FromQuery] int year = 0)
    {
        if (year == 0)
        {
            year = DateTime.UtcNow.Year;
        }

        if (year < 2000 || year > DateTime.UtcNow.Year)
        {
            return BadRequest("Invalid year provided");
        }

        var report = await _reportService.GetMonthlyPurchaseReportAsync(year);
        return Ok(report);
    }

    /// <summary>
    /// Get top selling parts report
    /// </summary>
    /// <param name="limit">Number of top parts to retrieve (default: 10)</param>
    [HttpGet("products/top-selling")]
    public async Task<ActionResult<IEnumerable<TopSellingPartResponse>>> GetTopSellingParts(
        [FromQuery] int limit = 10)
    {
        if (limit < 1 || limit > 100)
        {
            return BadRequest("Limit must be between 1 and 100");
        }

        var report = await _reportService.GetTopSellingPartsReportAsync(limit);
        return Ok(report);
    }

    /// <summary>
    /// Get inventory summary report
    /// </summary>
    [HttpGet("inventory/summary")]
    public async Task<ActionResult<InventorySummaryResponse>> GetInventorySummary()
    {
        var report = await _reportService.GetInventorySummaryAsync();
        return Ok(report);
    }

    private static DateTime EnsureUtc(DateTime value)
        => value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
}

