using Anbarak.Application.Features.Products;
using Microsoft.Extensions.DependencyInjection;

namespace Anbarak.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ProductService>();
            return services;
        }
    }
}
