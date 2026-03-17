using System.Diagnostics;
using Serilog.Core;
using Serilog.Events;

namespace CQRS.Pattern.Infrastructure.Logging;

public sealed class ShortCorrelationIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var traceId = Activity.Current?.TraceId.ToString();

        var shortTraceId = !string.IsNullOrEmpty(traceId)
            ? traceId[..8]
            : string.Empty;

        logEvent.AddPropertyIfAbsent(
            propertyFactory.CreateProperty("ShortCorrelationId", shortTraceId));
    }
}
