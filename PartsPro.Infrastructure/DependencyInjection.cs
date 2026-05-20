using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartsPro.Infrastructure.BackgroundJobs;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Services;
using PartsPro.Infrastructure.Data;
using PartsPro.Infrastructure.Repositories;
using PartsPro.Domain.Entities;

namespace PartsPro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IEmailRepository, EmailRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IStaffService, StaffService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<INotificationService, NotificationService>();

        // Background Services
        services.AddHostedService<NotificationBackgroundService>();

        return services;
    }
}