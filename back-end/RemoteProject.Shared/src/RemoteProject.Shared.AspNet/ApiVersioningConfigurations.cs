using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RemoteProject.Shared.AspNet;

public static class ApiVersioningConfigurations
{
    public static void AddAppApiVersioning(
        IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);

            // Uses 1.0 when there's no version information in the request.
            opt.AssumeDefaultVersionWhenUnspecified = true;

            // When you do this, ASP.NET Core provides an api-supported-versions response header that shows which versions an endpoint supports.
            opt.ReportApiVersions = true;

            var readers = new List<IApiVersionReader>
            {
                new MediaTypeApiVersionReader("v"),
                new HeaderApiVersionReader("X-Api-Version"),
                new QueryStringApiVersionReader("api-version")
            };

            // X-Api-Version used to keep openness to allow version selection in environments that prevent media type to be tampered with.
            opt.ApiVersionReader = ApiVersionReader.Combine(readers);
        });
    }
}
