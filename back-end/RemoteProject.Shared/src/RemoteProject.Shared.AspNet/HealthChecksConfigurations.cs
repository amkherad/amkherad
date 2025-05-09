using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteProject.Shared.AspNet;

public static class HealthChecksConfigurations
{
    public const string HealthCheckPath = "/health/status";

    public static IServiceCollection AddMateHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddHealthChecks();

        return services;
    }

    public static IApplicationBuilder UseMateHealthChecks(
        this IApplicationBuilder app,
        string path = HealthCheckPath
    )
    {
        app.UseHealthChecks(path);

        return app;
    }
}
