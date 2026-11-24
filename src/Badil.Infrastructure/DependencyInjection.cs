using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Badil.Application.Common.interfaces;
using Badil.Infrastructure.Services;

namespace Badil.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {

            service.AddScoped<IPingService , pingService>();
            return service;
        }
    }
}
