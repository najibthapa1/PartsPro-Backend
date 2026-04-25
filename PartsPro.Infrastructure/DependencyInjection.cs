using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PartsPro.Application.Interfaces;
using PartsPro.Application.Services;
using PartsPro.Infrastructure.Data;

namespace PartsPro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}