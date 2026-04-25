using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PartsPro.Domain.Entities;

namespace PartsPro.Infrastructure.Data;

public static class DbSeeder
{
    /// <summary>
    /// Seed initial roles and admin user
    /// Call this from Program.cs after migrations
    /// </summary>
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Admin User
            await SeedAdminAsync(userManager);
        }
    }

    /// <summary>
    /// Seed default roles: Admin, Staff, Customer
    /// </summary>
    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "Staff", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"✓ Role '{role}' created");
            }
        }
    }

    /// <summary>
    /// Seed initial admin user
    /// Email: admin@partspro.com
    /// Password: Admin@123
    /// </summary>
    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@partspro.com";
        const string adminPassword = "Admin@123";
        const string adminName = "System Administrator";

        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // Create admin user
            var admin = new ApplicationUser
            {
                Email = adminEmail,
                UserName = adminEmail,
                FullName = adminName,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, adminPassword);

            if (result.Succeeded)
            {
                // Assign Admin role
                await userManager.AddToRoleAsync(admin, "Admin");
                Console.WriteLine($"✓ Admin user created: {adminEmail}");
                Console.WriteLine($"  Password: {adminPassword}");
            }
            else
            {
                Console.WriteLine($"✗ Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            Console.WriteLine($"✓ Admin user already exists: {adminEmail}");
        }
    }
}

