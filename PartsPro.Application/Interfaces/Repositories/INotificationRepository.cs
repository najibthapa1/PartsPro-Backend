using PartsPro.Domain.Entities;

namespace PartsPro.Application.Interfaces.Repositories;

/// <summary>
/// Repository for notification data queries (low stock parts, overdue credits)
/// </summary>
public interface INotificationRepository
{
    /// <summary>
    /// Get all parts with stock below threshold
    /// </summary>
    Task<List<Part>> GetLowStockPartsAsync(int threshold = 10);

    /// <summary>
    /// Get all customers with overdue unpaid credits
    /// </summary>
    Task<List<(Customer Customer, decimal TotalOverdueAmount, int DaysOverdue)>> GetCustomersWithOverdueCreditsAsync(int daysThreshold = 30);
}

