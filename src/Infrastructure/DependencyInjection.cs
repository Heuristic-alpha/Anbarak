using Anbarak.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Anbarak.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Anbarak")!;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string \'Anbarak\' was not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options => 
        { 
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
