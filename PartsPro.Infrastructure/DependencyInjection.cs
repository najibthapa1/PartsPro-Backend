using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        //services for vendor management
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IVendorService, VendorService>();
        
        return services;
    }
}