namespace PartsPro.Application.DTOs.Reports;

/// <summary>
/// DTO for comprehensive financial report data
/// </summary>
public class FinancialReportResponse
{
    public DateTime ReportDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Sales Metrics
    public decimal TotalSalesRevenue { get; set; }
    public int TotalSalesCount { get; set; }
    public decimal AverageSaleAmount { get; set; }
    public decimal TotalSalesDiscounts { get; set; }

    // Purchase Metrics
    public decimal TotalPurchaseCost { get; set; }
    public int TotalPurchasesCount { get; set; }
    public decimal AveragePurchaseAmount { get; set; }
    public decimal TotalPurchasesPaid { get; set; }
    public decimal TotalPurchasesUnpaid { get; set; }

    // Inventory Metrics
    public int TotalPartsInInventory { get; set; }
    public decimal TotalInventoryValue { get; set; }
    public int LowStockPartsCount { get; set; }

    // Financial Summary
    public decimal GrossProfit { get; set; }
    public decimal NetProfit { get; set; }
    public decimal CostOfGoodsSold { get; set; }
    public decimal GrossProfitMargin { get; set; }

    // Customer Metrics
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public decimal AverageOrderValue { get; set; }

    // Vendor Metrics
    public int TotalVendors { get; set; }
    public int ActiveVendors { get; set; }
    public decimal AverageVendorPayment { get; set; }
}

