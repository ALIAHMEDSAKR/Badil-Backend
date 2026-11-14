using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Badil.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            return service;
        }
    }
}
