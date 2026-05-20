namespace PartsPro.Application.Interfaces.Services;

/// <summary>
/// Service for handling system notifications (low stock, overdue credits, etc.)
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Check for low stock parts and notify admin
    /// </summary>
    Task NotifyLowStockAsync();

    /// <summary>
    /// Send email reminders to customers with overdue credits (>30 days unpaid)
    /// </summary>
    Task SendOverdueCreditsReminderAsync();

    /// <summary>
    /// Run all notification jobs
    /// </summary>
    Task ProcessAllNotificationsAsync();
}

