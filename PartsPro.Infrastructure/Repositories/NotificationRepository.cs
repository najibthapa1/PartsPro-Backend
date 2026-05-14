using Microsoft.EntityFrameworkCore;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Domain.Entities;
using PartsPro.Domain.Enums;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure.Repositories;

/// <summary>
/// Notification repository - queries for low stock parts and overdue credits
/// </summary>
public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Get all parts with stock below threshold
    /// </summary>
    public async Task<List<Part>> GetLowStockPartsAsync(int threshold = 10)
    {
        return await _context.Parts
            .Where(p => p.Stock < threshold)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Get all customers with overdue unpaid credits (threshold in days)
    /// </summary>
    public async Task<List<(Customer Customer, decimal TotalOverdueAmount, int DaysOverdue)>> GetCustomersWithOverdueCreditsAsync(int daysThreshold = 30)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);

        var overdueCredits = await _context.CreditRecords
            .Where(c => c.Status == InvoiceStatus.Unpaid && c.CreatedAt <= thresholdDate)
            .Include(c => c.Customer)
            .ThenInclude(cu => cu.User)
            .AsNoTracking()
            .ToListAsync();

        var groupedByCustomer = overdueCredits
            .GroupBy(c => c.Customer)
            .Select(g => (
                Customer: g.Key,
                TotalOverdueAmount: g.Select(c => c.Amount).Sum(),
                DaysOverdue: (int)(DateTime.UtcNow - g.Min(c => c.CreatedAt)).TotalDays
            ))
            .ToList();

        return groupedByCustomer;
    }
}

