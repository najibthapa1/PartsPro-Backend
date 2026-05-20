using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PartsPro.Application.DTOs.Customers;
using PartsPro.Application.DTOs.Reports;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;
    private readonly ICustomerService _customerService;

    public ReportController(IReportService reportService, ICustomerService customerService)
    {
        _reportService = reportService;
        _customerService = customerService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("financial")]
    public async Task<ActionResult<object>> GetFinancialReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);
        if (startDate > endDate) return BadRequest("Start date must be before end date");
        var report = await _reportService.GetFinancialReportAsync(startDate, endDate);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("profit-loss")]
    public async Task<ActionResult<object>> GetProfitAndLoss([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);
        if (startDate > endDate) return BadRequest("Start date must be before end date");
        var report = await _reportService.GetProfitAndLossStatementAsync(startDate, endDate);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("financial/yearly")]
    public async Task<ActionResult<object>> GetYearlyFinancialReport([FromQuery] int year = 0)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (year < 2000 || year > DateTime.UtcNow.Year) return BadRequest("Invalid year provided");
        var report = await _reportService.GetYearlyFinancialReportAsync(year);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("sales/monthly")]
    public async Task<ActionResult<object>> GetMonthlySalesReport([FromQuery] int year = 0)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (year < 2000 || year > DateTime.UtcNow.Year) return BadRequest("Invalid year provided");
        var report = await _reportService.GetMonthlySalesReportAsync(year);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("sales/daily")]
    public async Task<ActionResult<object>> GetDailySalesReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        startDate = EnsureUtc(startDate);
        endDate = EnsureUtc(endDate);
        if (startDate > endDate) return BadRequest("Start date must be before end date");
        var report = await _reportService.GetDailySalesReportAsync(startDate, endDate);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("purchases/monthly")]
    public async Task<ActionResult<object>> GetMonthlyPurchaseReport([FromQuery] int year = 0)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (year < 2000 || year > DateTime.UtcNow.Year) return BadRequest("Invalid year provided");
        var report = await _reportService.GetMonthlyPurchaseReportAsync(year);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("products/top-selling")]
    public async Task<ActionResult<object>> GetTopSellingParts([FromQuery] int limit = 10)
    {
        if (limit < 1 || limit > 100) return BadRequest("Limit must be between 1 and 100");
        var report = await _reportService.GetTopSellingPartsReportAsync(limit);
        return Ok(report);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("inventory/summary")]
    public async Task<ActionResult<object>> GetInventorySummary()
    {
        var report = await _reportService.GetInventorySummaryAsync();
        return Ok(report);
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customer-insights")]
    public async Task<ActionResult<CustomerInsightSummaryResponse>> GetCustomerInsights()
    {
        var insights = await _customerService.GetCustomerInsightsAsync();
        return Ok(insights);
    }

    private static DateTime EnsureUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
