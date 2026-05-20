using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PartsPro.Application.Interfaces.Services;

namespace PartsPro.Infrastructure.BackgroundJobs;

/// <summary>
/// Background service that runs notification jobs on a scheduled interval
/// </summary>
public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1); 

    public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Notification background service is starting");

        using var timer = new PeriodicTimer(_interval);

        try
        {
            await ProcessNotificationsAsync(stoppingToken);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessNotificationsAsync(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Notification background service is stopping");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Notification background service encountered an error");
        }
    }

    private async Task ProcessNotificationsAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            await notificationService.ProcessAllNotificationsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing notifications");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Notification background service is stopping");
        await base.StopAsync(cancellationToken);
    }
}

