using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Domain.Entities;
using PartsPro.Domain.Enums;
using PartsPro.Application.Interfaces.Repositories;

namespace PartsPro.Application.Services;

/// <summary>
/// Service for system notifications: low-stock alerts and overdue-credit reminders
/// Feature 15: Automatically notify Admin for low stock and email reminders for overdue credits
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IEmailRepository _emailRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IEmailRepository emailRepository,
        UserManager<ApplicationUser> userManager,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _emailRepository = emailRepository;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Check for low stock parts (stock < 10) and send email to all admin users
    /// </summary>
    public async Task NotifyLowStockAsync()
    {
        try
        {
            var lowStockParts = await _notificationRepository.GetLowStockPartsAsync();

            if (!lowStockParts.Any())
            {
                _logger.LogInformation("No low-stock parts found");
                return;
            }

            // Get all admin users
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");

            if (!adminUsers.Any())
            {
                _logger.LogWarning("No admin users found to notify about low stock");
                return;
            }

            // Build email body
            var emailBody = BuildLowStockEmailBody(lowStockParts);

            // Send to all admins
            var adminEmails = adminUsers
                .Where(u => !string.IsNullOrWhiteSpace(u.Email))
                .Select(u => (u.Email!, "⚠️ Low Stock Alert - PartsPro", emailBody))
                .ToList();

            var sentCount = await _emailRepository.SendBulkEmailAsync(adminEmails, isHtml: true);
            _logger.LogInformation("Low stock notification sent to {SentCount}/{TotalCount} admins. Parts affected: {PartCount}", 
                sentCount, adminEmails.Count, lowStockParts.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying low stock");
        }
    }

    /// <summary>
    /// Send email reminders to customers with unpaid credit balances overdue by more than 30 days
    /// </summary>
    public async Task SendOverdueCreditsReminderAsync()
    {
        try
        {
            var customersWithOverdueCredits = await _notificationRepository.GetCustomersWithOverdueCreditsAsync();

            if (!customersWithOverdueCredits.Any())
            {
                _logger.LogInformation("No overdue credits found");
                return;
            }

            var emailsToSend = new List<(string Email, string Subject, string Body)>();

            foreach (var (customer, totalOverdueAmount, daysOverdue) in customersWithOverdueCredits)
            {
                if (string.IsNullOrWhiteSpace(customer.User?.Email))
                    continue;

                var emailBody = BuildOverdueCreditEmailBody(customer, totalOverdueAmount, daysOverdue);
                emailsToSend.Add((customer.User.Email, "⏰ Outstanding Credit Balance Reminder - PartsPro", emailBody));
            }

            if (emailsToSend.Any())
            {
                var sentCount = await _emailRepository.SendBulkEmailAsync(emailsToSend, isHtml: true);
                _logger.LogInformation("Overdue credit reminders sent to {SentCount}/{TotalCount} customers", sentCount, emailsToSend.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending overdue credit reminders");
        }
    }

    /// <summary>
    /// Run all scheduled notification tasks
    /// </summary>
    public async Task ProcessAllNotificationsAsync()
    {
        _logger.LogInformation("Starting scheduled notification batch at {Time}", DateTime.UtcNow);

        await NotifyLowStockAsync();
        await SendOverdueCreditsReminderAsync();

        _logger.LogInformation("Completed scheduled notification batch at {Time}", DateTime.UtcNow);
    }

    // Helper methods for email body generation
    private string BuildLowStockEmailBody(List<Part> lowStockParts)
    {
        var partsList = string.Join("</li><li>", lowStockParts.Select(p => $"{p.Name} (Part #: {p.PartNumber}) - Current Stock: {p.Stock}"));

        return $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #d32f2f;'>⚠️ Low Stock Alert</h2>
    <p>The following parts have stock levels below 10 units:</p>
    <ul style='line-height: 1.8;'>
        <li>{partsList}</li>
    </ul>
    <p style='color: #666;'>
        Please review and replenish inventory as needed.<br/>
        <strong>Action Required:</strong> Contact vendors to reorder stock.
    </p>
    <hr/>
    <p style='font-size: 12px; color: #999;'>
        This is an automated message from PartsPro. Please do not reply to this email.
    </p>
</body>
</html>";
    }

    private string BuildOverdueCreditEmailBody(Customer customer, decimal totalAmount, int daysOverdue)
    {
        return $@"
<html>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #ff9800;'>⏰ Outstanding Credit Balance Reminder</h2>
    <p>Dear {customer.FullName},</p>
    <p>We have a record of an outstanding credit balance on your account:</p>
    
    <div style='background-color: #f5f5f5; padding: 15px; border-left: 4px solid #ff9800;'>
        <p><strong>Total Outstanding Amount:</strong> <span style='font-size: 18px; color: #d32f2f;'>₹{totalAmount:F2}</span></p>
        <p><strong>Days Overdue:</strong> {daysOverdue} days</p>
    </div>
    
    <p>We kindly request payment at your earliest convenience to avoid any inconvenience.</p>
    <p>If you have already made payment, please disregard this message.</p>
    <p>
        For any questions or to arrange a payment, please contact us.<br/>
        Thank you for your business!
    </p>
    <hr/>
    <p style='font-size: 12px; color: #999;'>
        This is an automated message from PartsPro. Please do not reply to this email.
    </p>
</body>
</html>";
    }
}

