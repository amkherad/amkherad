using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteProject.Shared.AspNet;

public static class Configurations
{
    public static IServiceCollection AddConfig<T>(
        this IServiceCollection services,
        IConfiguration configuration
    )
        where T : class
    {
        services.Configure<T>(opt => configuration.GetSection(typeof(T).Name).Bind(opt));

        return services;
    }

    public static IServiceCollection AddConfig<T>(
        this IServiceCollection services,
        IConfiguration configuration,
        out T config
    )
        where T : class
    {
        config = configuration.GetSection(typeof(T).Name).Get<T>();

        services.Configure<T>(opt => configuration.GetSection(typeof(T).Name).Bind(opt));

        return services;
    }

    public static IApplicationBuilder FillInServerAddresses(
        this IApplicationBuilder app,
        IConfiguration configuration
    )
    {
        try
        {
            var section = configuration.GetSection("Urls");

            string? urlsStr = null;

            if (section is not null)
            {
                urlsStr = section.Get<string>();
            }

            string[] urls;
            if (urlsStr is not null && urlsStr.Length > 0)
            {
                urls = urlsStr.Split(';')
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Replace("//*:", "//0.0.0.0:").Replace("//+:", "//0.0.0.0:"))
                    .ToArray();
            }
            else
            {
                urls = new[] { "http://localhost:5000", "https://localhost:5001" };
            }

            var features = app.ServerFeatures;
            if (features is null) return app;
            var addresses = features.Get<IServerAddressesFeature>();
            if (addresses is null || addresses.Addresses is null) return app;
            if (addresses.Addresses.Count == 0)
            {
                foreach (var url in urls)
                {
                    addresses.Addresses.Add(url);
                }
            }
        }
        catch { }

        return app;
    }
}
