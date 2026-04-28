using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartsPro.Application.Interfaces.Services;
using PartsPro.Application.Interfaces.Repositories;
using PartsPro.Application.Services;
using PartsPro.Infrastructure.Data;
using PartsPro.Infrastructure.Repositories;

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
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IStaffRepository, StaffRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<IPartRepository, PartRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ISaleService, SaleService>();

        return services;
    }
}