using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using RemoteProject.Shared.AspNet.Helpers;
using RemoteProject.Shared.AspNet.Models;

namespace RemoteProject.Shared.AspNet.ServiceMesh;

public static class ServiceDiscoveryConfigurations
{
    public static IServiceCollection AddMateServiceDiscovery(
        this IServiceCollection services,
        ServiceDiscoveryConfig? config
    )
    {
        if (config is null || !config.IsEnabled)
        {
            return services;
        }

        services.AddSingleton<IConsulClient, ConsulClient>(sp => new ConsulClient(clientConfig =>
        {
            clientConfig.Address = InternalHelpers.BuildServiceUri(
                config.DiscoveryServiceHost,
                config.DiscoveryServicePort
            );
        }));

        services.AddScoped<ConsulRegistryService>();
        services.AddHostedService<ConsulRegistryService>();

        return services;
    }

    public static IApplicationBuilder UseMateServiceDiscovery(
        this IApplicationBuilder app
    )
    {
        return app;
    }
}
