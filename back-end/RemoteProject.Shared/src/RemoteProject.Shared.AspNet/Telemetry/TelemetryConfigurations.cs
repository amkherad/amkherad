using System.Diagnostics;
using System.Net.Sockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RemoteProject.Shared.AspNet.Models;
using Serilog;
using Serilog.Context;
using Serilog.Formatting.Json;

namespace RemoteProject.Shared.AspNet.Telemetry;

public static class TelemetryConfigurations
{
    private static OpenTelemetryBuilder AddMateLogging(
        this OpenTelemetryBuilder builder,
        IConfiguration configuration,
        TelemetryConfig config,
        OpenTelemetryLoggingConfig? loggingConfig,
        Action<OpenTelemetryLoggerOptions>? configurator = null
    )
    {
        if (loggingConfig is null)
        {
            return builder;
        }

        // builder.Host.ConfigureLogging(logging => logging
        //     .ClearProviders()
        //     .AddOpenTelemetry(options =>
        //     {
        //         options.IncludeFormattedMessage = true;
        //         options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MateMachine.OrderMicroservice"));
        //         options.AddConsoleExporter();
        //         options.AddJsonFileExporter();
        //         options.AddOtlpExporter(o => { o.Endpoint = new Uri("http://localhost:4317"); });
        //     }));

        return builder;
    }

    private static OpenTelemetryBuilder AddMateMetrics(
        this OpenTelemetryBuilder builder,
        TelemetryConfig config,
        OpenTelemetryMetricsConfig? metricConfig,
        Action<MeterProviderBuilder>? configurator = null
    )
    {
        if (metricConfig is null)
        {
            return builder;
        }

        return builder.WithMetrics(metrics =>
        {
            var resourceBuilder = ResourceBuilder.CreateDefault()
                .AddService(serviceName: config.ServiceName, serviceVersion: config.ServiceVersion);
            if (config.AdditionalAttributes is not null)
                resourceBuilder.AddAttributes(config.AdditionalAttributes);
            metrics.SetResourceBuilder(resourceBuilder);
            if (metricConfig.Meters is not null)
            {
                metrics.AddMeter(metricConfig.Meters);
            }
            

            metrics.AddAspNetCoreInstrumentation()
                .AddRuntimeInstrumentation()
                .AddHttpClientInstrumentation();

            if (metricConfig.IsProcessMetricsEnabled)
            {
                metrics.AddProcessInstrumentation();
            }

            if (metricConfig.UseConsoleExplorer)
            {
                metrics.AddConsoleExporter();
            }

            if (metricConfig.OtlpExporterEndpoint is not null)
            {
                metrics.AddOtlpExporter(o =>
                {
                    o.Endpoint = new Uri(metricConfig.OtlpExporterEndpoint);
                    //o.Protocol = OtlpExportProtocol.Grpc;
                    //o.ExportProcessorType = ExportProcessorType.Simple;
                });
            }

            configurator?.Invoke(metrics);
        });
    }

    private static OpenTelemetryBuilder AddMateTracing(
        this OpenTelemetryBuilder builder,
        TelemetryConfig config,
        OpenTelemetryTracingConfig? tracingConfig,
        Action<TracerProviderBuilder>? configurator = null
    )
    {
        if (tracingConfig is null)
        {
            return builder;
        }

        return builder
            .WithTracing(opt =>
            {
                var resourceBuilder = ResourceBuilder.CreateDefault()
                    .AddService(serviceName: config.ServiceName, serviceVersion: config.ServiceVersion);
                if (config.AdditionalAttributes is not null)
                    resourceBuilder.AddAttributes(config.AdditionalAttributes);
                opt
                    .SetResourceBuilder(resourceBuilder)
                    //.AddMassTransitInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddGrpcClientInstrumentation();

                if (tracingConfig.UseConsoleExplorer)
                {
                    opt.AddConsoleExporter();
                }


                if (tracingConfig.IsMassTransitTraceEnabled)
                    opt.AddSource(config.ServiceId, "MassTransit");

                if (tracingConfig.IsSqlTraceEnabled)
                {
                    opt.AddSqlClientInstrumentation(instrumentationOptions =>
                    {
                        if (tracingConfig.SqlTracingConfig is null) return;

                        instrumentationOptions.SetDbStatementForStoredProcedure =
                            tracingConfig.SqlTracingConfig.SetDbStatementForStoredProcedure;
                        instrumentationOptions.SetDbStatementForText = tracingConfig.SqlTracingConfig.SetDbStatementForText;
                        instrumentationOptions.EnableConnectionLevelAttributes =
                            tracingConfig.SqlTracingConfig.EnableConnectionLevelAttributes;
                    });
                }

                if (tracingConfig.IsHangfireTraceEnabled)
                {
                    opt.AddHangfireInstrumentation();
                }

                if (tracingConfig.IsStackExchangeTraceEnabled)
                {
                    opt.AddRedisInstrumentation();
                }

                if (tracingConfig.OtlpExporterEndpoint is not null)
                {
                    opt.AddOtlpExporter(o =>
                    {
                        o.Endpoint = new Uri(tracingConfig.OtlpExporterEndpoint);
                        //o.Protocol = OtlpExportProtocol.Grpc;
                        //o.ExportProcessorType = ExportProcessorType.Simple;
                    });
                }

                configurator?.Invoke(opt);
            });
    }

    private static IServiceCollection AddMateSerilog(
        this IServiceCollection services,
        IConfiguration configuration,
        TelemetryConfig config
    )
    {
        if (!(config.Serilog?.IsEnabled ?? false))
        {
            return services;
        }


        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration, config.Serilog.SectionName ?? "Serilog")
            .Enrich.FromLogContext();

        if (config.IsDataDogUsed)
        {
            loggerConfig.Enrich.With<OpenTelemetryToDataDogLogEnricher>();
            loggerConfig.Enrich.WithProperty("dd.service", config.ServiceName);
            loggerConfig.Enrich.WithProperty("dd.version", config.ServiceVersion);
        }
        else
        {
            loggerConfig.Enrich.WithProperty("ServiceName", config.ServiceName ?? AppDomain.CurrentDomain.FriendlyName);
            loggerConfig.Enrich.WithProperty("ServiceId", config.ServiceId ?? AppDomain.CurrentDomain.FriendlyName);
        }
        
        if (config.AdditionalAttributes is not null)
            foreach (var additionalAttribute in config.AdditionalAttributes)
            {
                loggerConfig.Enrich.WithProperty(additionalAttribute.Key, additionalAttribute.Value);
            }

        if (config.Serilog.WriteToFileConfig is not null)
            loggerConfig.WriteTo.File(new JsonFormatter(renderMessage: true),
                config.Serilog.WriteToFileConfig.Path,
                shared: config.Serilog.WriteToFileConfig.IsShared);

        if (config.Serilog.WriteToNetworkConfig is not null)
        {
            var networkConfig = config.Serilog.WriteToNetworkConfig;

            loggerConfig.WriteTo.Udp(networkConfig.Address,
                networkConfig.Port,
                AddressFamily.InterNetwork,
                formatter: new JsonFormatter(renderMessage: true));
        }

        var logger = loggerConfig.CreateLogger();
        Log.Logger = logger;

        return services.AddLogging(opt =>
        {
            opt.ClearProviders();
            opt.AddSerilog(logger);
        });
    }

    private static IServiceCollection AddMateOpenTelemetry(
        this IServiceCollection services,
        TelemetryConfig config,
        Action<TracerProviderBuilder>? tracerConfigurator = null,
        Action<MeterProviderBuilder>? meterConfigurator = null,
        Action<OpenTelemetryLoggerOptions>? loggerConfigurator = null
    )
    {
        if (config.OpenTelemetry is not null && config.OpenTelemetry.IsEnabled)
        {
            services
                .AddOpenTelemetry()
                .AddMateTracing(config, config.OpenTelemetry.Tracing, tracerConfigurator)
                .AddMateMetrics(config, config.OpenTelemetry.Metrics, meterConfigurator);
            // .AddMateLogging(config, null, null, loggerConfigurator);
        }

        return services;
    }

    public static IServiceCollection AddMateTelemetryServices(
        this IServiceCollection services,
        IConfiguration configuration,
        TelemetryConfig? config,
        Action<TracerProviderBuilder>? tracerConfigurator = null,
        Action<MeterProviderBuilder>? meterConfigurator = null,
        Action<OpenTelemetryLoggerOptions>? loggerConfigurator = null
    )
    {
        if (config is null || !config.IsEnabled)
        {
            return services;
        }

        return services
            .AddMateOpenTelemetry(config, tracerConfigurator, meterConfigurator, loggerConfigurator)
            .AddMateSerilog(configuration, config);
    }

    public static IApplicationBuilder UseMateTelemetryServices(
        this IApplicationBuilder app
    )
    {
        var config = app.ApplicationServices.GetRequiredService<IOptions<TelemetryConfig>>();

        if (!config.Value.IsEnabled)
        {
            return app;
        }

        if (config.Value.Serilog?.IsEnabled ?? false)
        {
            if (config.Value.Serilog.RequestLogging)
            {
                app.UseSerilogRequestLogging();
            }
        }

        return app;
    }

    public static IHostBuilder UseMateTelemetryServices(
        this IHostBuilder host,
        TelemetryConfig config
    )
    {
        ArgumentNullException.ThrowIfNull(config);

        if (!config.IsEnabled)

        {
            return host;
        }

        if (config.OpenTelemetry?.Logging is not null)
        {
            var serviceEnvironment = config.IsDataDogUsed ? "dd.env" : "env";

            host.ConfigureLogging(logging => logging
                .ClearProviders()
                .AddOpenTelemetry(opt =>
                {
                    var resourceBuilder = ResourceBuilder.CreateDefault()
                        .AddService(serviceName: config.ServiceName, serviceVersion: config.ServiceVersion);
                    if (config.AdditionalAttributes is not null)
                        resourceBuilder.AddAttributes(config.AdditionalAttributes);
                    
                    opt.SetResourceBuilder(resourceBuilder);

                    opt.IncludeFormattedMessage = config.OpenTelemetry.Logging.IncludeFormattedMessage;

                    opt.IncludeScopes = config.OpenTelemetry.Logging.IncludeScope;

                    opt.ParseStateValues = config.OpenTelemetry.Logging.ParseStateValue;

                    if (config.OpenTelemetry.Logging.UseConsoleExplorer)
                    {
                        opt.AddConsoleExporter();
                    }

                    if (config.OpenTelemetry.Logging.OtlpExporterEndpoint is not null)
                    {
                        opt.AddOtlpExporter(o =>
                        {
                            o.Endpoint = new Uri(config.OpenTelemetry.Logging.OtlpExporterEndpoint);
                            //o.Protocol = OtlpExportProtocol.Grpc;
                        });
                    }
                })
            );
        }

        if (config.Serilog?.IsEnabled ?? false)
        {
            return host.UseSerilog(Log.Logger);
        }

        return host;
    }
}