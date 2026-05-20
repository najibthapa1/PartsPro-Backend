namespace PartsPro.Application.DTOs.Reports;

/// <summary>
/// DTO for monthly sales summary
/// </summary>
public class MonthlySalesResponse
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalRevenue { get; set; }
    public int SalesCount { get; set; }
    public decimal AverageOrderValue { get; set; }
}

/// <summary>
/// DTO for monthly purchase summary
/// </summary>
public class MonthlyPurchaseResponse
{
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal TotalCost { get; set; }
    public int PurchaseCount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal UnpaidAmount { get; set; }
}

/// <summary>
/// DTO for top selling parts report
/// </summary>
public class TopSellingPartResponse
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public string PartNumber { get; set; } = string.Empty;
    public int TotalQuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}

/// <summary>
/// DTO for inventory summary report
/// </summary>
public class InventorySummaryResponse
{
    public int TotalPartsInStock { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
    public List<LowStockPartResponse> LowStockParts { get; set; } = new();
}

/// <summary>
/// DTO for low stock parts
/// </summary>
public class LowStockPartResponse
{
    public int PartId { get; set; }
    public string PartName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int ReorderLevel { get; set; }
}

/// <summary>
/// DTO for daily sales summary
/// </summary>
public class DailySalesResponse
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public int SalesCount { get; set; }
}

