using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RemoteProject.Shared.AspNet.Models;

namespace RemoteProject.Shared.AspNet;

public static class CorsConfigurations
{
    public static IServiceCollection AddAppCors(
        this IServiceCollection services,
        CorsConfig? config
    ) => AddAppCors(services, null, config);

    public static IServiceCollection AddAppCors(
        this IServiceCollection services,
        ILogger? logger,
        CorsConfig? config
    )
    {
        var isEnabled = config?.IsEnabled ?? false;
        logger?.LogInformation(
            "@AddMateCors, IsEnabled:{IsEnabled}",
            isEnabled
        );

        if (config is null || !isEnabled)
        {
            return services;
        }

        var allowCredentials = config.AllowCredentials ?? false;
        var allowAnyHeader = config.AllowAnyHeader ?? false;
        var allowAnyMethod = config.AllowAnyMethod ?? false;
        var allowAnyOrigin = config.AllowAnyOrigin ?? false;
        var isDefaultPolicy = config.IsDefaultPolicy ?? false;
        string? origins = string.Empty;
        string? headers = string.Empty;
        string? methods = string.Empty;
        string? exposedHeaders = string.Empty;

        services.AddCors(opt =>
        {
            var policyBuilder = new CorsPolicyBuilder();

            if (allowCredentials) policyBuilder.AllowCredentials();
            if (allowAnyHeader) policyBuilder.AllowAnyHeader();
            if (allowAnyMethod) policyBuilder.AllowAnyMethod();
            if (allowAnyOrigin) policyBuilder.AllowAnyOrigin();

            if (config.Origins is not null && !allowAnyOrigin)
            {
                policyBuilder.WithOrigins(config.Origins);
                origins = string.Join(",", config.Origins);
            }

            if (config.Headers is not null && !allowAnyHeader)
            {
                policyBuilder.WithHeaders(config.Headers);
                headers = string.Join(",", config.Headers);
            }

            if (config.Methods is not null && !allowAnyMethod)
            {
                policyBuilder.WithMethods(config.Methods);
                methods = string.Join(",", config.Methods);
            }

            if (config.ExposedHeaders is not null)
            {
                policyBuilder.WithExposedHeaders(config.ExposedHeaders);
                exposedHeaders = string.Join(",", config.ExposedHeaders);
            }

            var policy = policyBuilder.Build();

            if (!string.IsNullOrWhiteSpace(config.PolicyName))
            {
                opt.AddPolicy(config.PolicyName, policy);
            }

            if (isDefaultPolicy)
            {
                opt.AddDefaultPolicy(policy);
            }
        });

        logger?.LogInformation(
            "[SHA] @AddMateCors, Registered CORS, IsEnabled:{IsEnabled}, AllowCredentials:{AllowCredentials}, " +
            "AllowAnyHeader:{AllowAnyHeader}, AllowAnyMethod:{AllowAnyMethod}, AllowAnyOrigin:{AllowAnyOrigin}, " +
            "Origin:{Origin}, Headers:{Headers}, Methods:{Methods}, ExposedHeaders:{ExposedHeaders}, " +
            "PolicyName:{PolicyName}, IsDefaultPolicy:{IsDefaultPolicy}",
            isEnabled,
            allowCredentials,
            allowAnyHeader,
            allowAnyMethod,
            allowAnyOrigin,
            origins,
            headers,
            methods,
            exposedHeaders,
            config.PolicyName,
            config.IsDefaultPolicy
        );

        return services;
    }
}