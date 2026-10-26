using Anbarak.Application.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Anbarak.Infrastructure.Identity;

public class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IConfiguration configuration, bool allowDevelopmentAdminBootstrap)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roleNames = new string[]
        {
            IdentityRoles.Administrator,
            IdentityRoles.WarehouseManager,
            IdentityRoles.WarehouseOprator,
            IdentityRoles.WarehouseVisitor,
        };

        foreach ( var roleName in roleNames)
        {
            if(! await roleManager.RoleExistsAsync(roleName))
            {
                IdentityRole role = new IdentityRole(roleName);
                var result = await roleManager.CreateAsync(role);

                EnsureSucceeded(result, $"create new role({roleName})");
            }
        }

        if (!allowDevelopmentAdminBootstrap) return;

        string? email = configuration.GetValue<string>("BootstrapAdmin:Email");
        string? password = configuration.GetValue<string>("BootstrapAdmin:Password");

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException("Both BootstrapAdmin:Email and BootstrapAdmin:Password must be configured together.");
        }

        // Bootstrap account creation is optional.
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(password)) return;

        var adminUser = await userManager.FindByEmailAsync(email);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var adminCreationResult = await userManager.CreateAsync(adminUser, password);
            EnsureSucceeded(adminCreationResult, $"create admin user ({email})");
        }

        if (!await userManager.IsInRoleAsync(adminUser, IdentityRoles.Administrator))
        {
            var roleResult = await userManager.AddToRoleAsync(adminUser, IdentityRoles.Administrator);

            EnsureSucceeded(roleResult, "assign the Administrator role");
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string operationMessage)
    {
        if (result.Succeeded) return;

        string joinErrors = string.Join("; ", result.Errors);
        throw new InvalidOperationException($"Could not {operationMessage}: {joinErrors}");
    }
}
