using IdentityServer4.AccessTokenValidation;
using RemoteProject.Shared.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RemoteProject.Shared.AspNet.Models;

namespace RemoteProject.Shared.AspNet;

public static class IdentityConfiguration
{
    public const string LegacyXAuthScheme = "XAuth";
    public const string LegacyXAuthHeaderName = "X-MATEAPI-AUTH";

    public static IServiceCollection AddMateLegacyIdentity(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthorizationOptions>? authorizationConfigurator = null,
        Action<AuthenticationOptions>? authenticationConfigurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        return services
            .AddMateLegacyAuthorization(environment, config, authorizationConfigurator)
            .AddMateLegacyAuthentication(environment, config, authenticationConfigurator);
    }

    public static IServiceCollection AddMateIdentity(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthorizationOptions>? authorizationConfigurator = null,
        Action<AuthenticationOptions>? authenticationConfigurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        return services
            .AddMateAuthorization(environment, config, authorizationConfigurator)
            .AddMateAuthentication(environment, config, authenticationConfigurator);
    }

    public static IServiceCollection AddMateAuthorization(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthorizationOptions>? configurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddAuthorization(options =>
        {
            var builder = new AuthorizationPolicyBuilder(IdentityServerAuthenticationDefaults.AuthenticationScheme);
            builder.RequireAuthenticatedUser();

            var userPolicy = builder.Build();
            options.DefaultPolicy = userPolicy;

            options.AddPolicy(AuthConstants.UserPolicy, userPolicy);

            options.AddPolicy(AuthConstants.Administration.AdminPolicy, builder =>
            {
                builder.AuthenticationSchemes.Add(IdentityServerAuthenticationDefaults.AuthenticationScheme);
                builder.RequireAuthenticatedUser();
                builder.RequireRole(AuthConstants.Administration.AdminRole);
            });

            configurator?.Invoke(options);
        });

        return services;
    }

    public static IServiceCollection AddMateLegacyAuthorization(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthorizationOptions>? configurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddAuthorization(options =>
        {
            var builder = new AuthorizationPolicyBuilder(LegacyXAuthScheme);
            builder.RequireAuthenticatedUser();

            var userPolicy = builder.Build();
            options.DefaultPolicy = userPolicy;

            options.AddPolicy(AuthConstants.UserPolicy, userPolicy);

            options.AddPolicy(AuthConstants.Administration.AdminPolicy, builder =>
            {
                builder.AuthenticationSchemes.Add(LegacyXAuthScheme);
                builder.RequireAuthenticatedUser();
                builder.RequireRole(AuthConstants.Administration.AdminRole);
            });

            configurator?.Invoke(options);
        });

        return services;
    }

    public static IServiceCollection AddMateAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthenticationOptions>? configurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;

                configurator?.Invoke(options);
            })
            .AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Authority = config.AuthorityUrl;
                options.RequireHttpsMetadata = false;

                options.SaveToken = config.SaveToken ?? false;
                options.Audience = config.Audience;
                options.TokenValidationParameters.ValidateAudience = false;
            });

        return services;
    }

    public static IServiceCollection AddMateLegacyAuthentication(
        this IServiceCollection services,
        IWebHostEnvironment environment,
        IdentityConfig config,
        Action<AuthenticationOptions>? configurator = null
    )
    {
        ArgumentNullException.ThrowIfNull(config, nameof(config));

        services.AddAuthentication(options =>
            {
                options.DefaultScheme = LegacyXAuthScheme;
                options.DefaultChallengeScheme = LegacyXAuthScheme;
                options.DefaultAuthenticateScheme = LegacyXAuthScheme;

                configurator?.Invoke(options);
            })
            .AddOAuth2Introspection(LegacyXAuthScheme, options =>
            {
                options.Authority = config.AuthorityUrl;
                options.DiscoveryPolicy.RequireHttps = config.RequireHttps ?? true;

                options.SaveToken = config.SaveToken ?? false;
                if (config.EnableCaching ?? false)
                {
                    options.EnableCaching = true;
                    options.CacheDuration = config.CacheDuration ?? TimeSpan.FromMinutes(10);
                }

                options.ClientId = config.ClientId;
                options.ClientSecret = config.ClientSecret;

                options.TokenRetriever = req => req.Headers[LegacyXAuthHeaderName];
            });

        return services;
    }
}
