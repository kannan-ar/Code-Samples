using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace DomainService.Infrastructure;

public class ActivityEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (Activity.Current is { } activity)
        {
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
            logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
        }
    }
}