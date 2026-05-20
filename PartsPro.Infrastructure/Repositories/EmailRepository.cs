using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MimeKit;
using PartsPro.Application.Interfaces.Repositories;

namespace PartsPro.Infrastructure.Repositories;

/// <summary>
/// Email repository - handles SMTP operations via MailKit
/// </summary>
public class EmailRepository : IEmailRepository
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailRepository> _logger;

    public EmailRepository(IConfiguration configuration, ILogger<EmailRepository> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"];
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromAddress = smtpSettings["FromAddress"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("SMTP settings not configured. Email not sent to {RecipientEmail}", recipientEmail);
                return false;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PartsPro System", fromAddress ?? username));
            message.To.Add(MailboxAddress.Parse(recipientEmail));
            message.Subject = subject;

            message.Body = new TextPart(isHtml ? "html" : "plain")
            {
                Text = body
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            _logger.LogInformation("Email sent successfully to {RecipientEmail}", recipientEmail);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {RecipientEmail}", recipientEmail);
            return false;
        }
    }

    public async Task<int> SendBulkEmailAsync(List<(string Email, string Subject, string Body)> emails, bool isHtml = true)
    {
        int successCount = 0;

        foreach (var (email, subject, body) in emails)
        {
            if (await SendEmailAsync(email, subject, body, isHtml))
            {
                successCount++;
            }
        }

        _logger.LogInformation("Bulk email sent: {SuccessCount}/{TotalCount} emails succeeded", successCount, emails.Count);
        return successCount;
    }
}

