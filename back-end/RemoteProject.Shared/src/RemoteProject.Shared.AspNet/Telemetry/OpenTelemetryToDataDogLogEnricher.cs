using System.Diagnostics;
using Serilog.Context;
using Serilog.Core;
using Serilog.Events;

namespace RemoteProject.Shared.AspNet.Telemetry;

public class OpenTelemetryToDataDogLogEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var stringTraceId = Activity.Current?.TraceId.ToString();
        var stringSpanId = Activity.Current?.SpanId.ToString();

        if (stringTraceId is not null)
        {
            var ddTraceId = Convert.ToUInt64(stringTraceId.Substring(16), 16).ToString();
            logEvent.AddPropertyIfAbsent(new LogEventProperty("dd.trace_id",new ScalarValue(ddTraceId)));
        }
        if (stringSpanId is not null)
        {
            var ddSpanId =Convert.ToUInt64(stringSpanId, 16).ToString();
            logEvent.AddPropertyIfAbsent(new LogEventProperty("dd.span_id",new ScalarValue(ddSpanId)));
        }
    }
}