namespace RemoteProject.Shared.AspNet.Models;

public class TelemetryConfig : AppServiceConfig
{
    public TimeSpan? FlushInterval { get; set; }

    public OpenTelemetryConfig? OpenTelemetry { get; set; }

    public SerilogConfig? Serilog { get; set; }

    public bool IsDataDogUsed { get; set; }

    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}

public class OpenTelemetryConfig
{
    public bool IsEnabled { get; set; }

    public OpenTelemetryMetricsConfig? Metrics { get; set; }
    public OpenTelemetryTracingConfig? Tracing { get; set; }
    public OpenTelemetryLoggingConfig? Logging { get; set; }
}

public class OpenTelemetryMetricsConfig
{
    public bool IsEnabled { get; set; }

    public string[]? Meters { get; set; }

    public string? OtlpExporterEndpoint { get; set; }

    public bool UseConsoleExplorer { get; set; }

    public bool IsProcessMetricsEnabled { get; set; }
}

public class OpenTelemetryTracingConfig
{
    public bool IsEnabled { get; set; }

    public string? OtlpExporterEndpoint { get; set; }

    public bool UseConsoleExplorer { get; set; }

    public bool IsSqlTraceEnabled { get; set; }

    public bool IsHangfireTraceEnabled { get; set; }

    public bool IsStackExchangeTraceEnabled { get; set; }

    public bool IsMassTransitTraceEnabled { get; set; }

    /// <summary>
    /// The SqlClientInstrumentationOptions class exposes two properties that can be used to configure how the db.
    /// statement attribute is captured upon execution of a query but the behavior depends on the runtime used.
    /// </summary>
    public OpenTelemetrySqlClientTracingConfig? SqlTracingConfig { get; set; }

}

public class OpenTelemetryLoggingConfig
{
    public bool IsEnabled { get; set; }

    public string? OtlpExporterEndpoint { get; set; }

    public bool UseConsoleExplorer { get; set; }

    public bool IncludeFormattedMessage { get; set; }

    public bool IncludeScope { get; set; }

    public bool ParseStateValue { get; set; }
}

public class SerilogConfig
{
    public bool IsEnabled { get; set; }

    public string? SectionName { get; set; }

    public bool RequestLogging { get; set; }

    public SerilogWriteToFileConfig? WriteToFileConfig { get; set; }
    public SerilogWriteToNetworkConfig? WriteToNetworkConfig { get; set; }

}


public class SerilogWriteToFileConfig
{
    public string Path { get; set; }
    public bool IsShared { get; set; }
}

public class SerilogWriteToNetworkConfig
{
    public string Address { get; set; }
    public bool IsIpAddress { get; set; }
    public int Port { get; set; }
}

public class OpenTelemetrySqlClientTracingConfig
{
    /// <summary>
    /// SetDbStatementForStoredProcedure is true by default and will set db.statement attribute to the stored procedure command name
    /// </summary>
    public bool SetDbStatementForStoredProcedure { get; set; } = true;

    /// <summary>
    /// SetDbStatementForText is false by default (to prevent accidental capture of sensitive data that might be part of the SQL statement text).
    /// When set to true, the instrumentation will set db.statement attribute to the text of the SQL command being executed.
    /// </summary>
    public bool SetDbStatementForText { get; set; }

    /// <summary>
    /// By default, EnabledConnectionLevelAttributes is disabled and this instrumentation sets the peer.service attribute to the DataSource property of the connection.
    /// If EnabledConnectionLevelAttributes is enabled, the DataSource will be parsed and the server name will be sent as the net.peer.name or net.peer.ip attribute,
    /// the instance name will be sent as the db.mssql.instance_name attribute, and the port will be sent as the net.peer.port attribute if it is not 1433 (the default port).
    /// </summary>
    public bool EnableConnectionLevelAttributes { get; set; }
}
