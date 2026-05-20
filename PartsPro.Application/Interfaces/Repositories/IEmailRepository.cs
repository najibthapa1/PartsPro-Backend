namespace PartsPro.Application.Interfaces.Repositories;

/// <summary>
/// Repository for email operations (infrastructure concern: SMTP sending)
/// </summary>
public interface IEmailRepository
{
    /// <summary>
    /// Send a single email
    /// </summary>
    Task<bool> SendEmailAsync(string recipientEmail, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Send bulk emails to multiple recipients
    /// </summary>
    Task<int> SendBulkEmailAsync(List<(string Email, string Subject, string Body)> emails, bool isHtml = true);
}

