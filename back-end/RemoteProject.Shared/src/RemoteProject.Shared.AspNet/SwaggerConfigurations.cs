using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using RemoteProject.Shared.AspNet.Models;

namespace RemoteProject.Shared.AspNet;

public static class SwaggerConfigurations
{
    public static IServiceCollection AddMateSwagger(
        this IServiceCollection services,
        SwaggerConfig config,
        Func<Assembly, bool>? assemblyFilter = null
    )
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!config.IsEnabled)
        {
            return services;
        }

        assemblyFilter ??= ass => ass.ManifestModule.Name.Contains(
            typeof(SwaggerConfigurations).Namespace?.Split('.').FirstOrDefault() ?? "Api"
        );

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc($"{config.ServiceId}_{config.Version}", new OpenApiInfo()
            {
                Version = config.Version,
                Title = config.Title,
                Description = config.Description,
            });

            var eligibleAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(assemblyFilter)
                .ToArray();

            if (eligibleAssemblies.Any())
            {
                foreach (var item in eligibleAssemblies)
                {
                    var fileName = $"{item.GetName().Name}.xml";
                    var filePath = Path.Combine(AppContext.BaseDirectory, fileName);
                    if (File.Exists(filePath))
                        option.IncludeXmlComments(filePath);
                }
            }
        });

        return services;
    }

    public static IApplicationBuilder UseMateSwagger(
        this IApplicationBuilder app
    )
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        return app;
    }
}
