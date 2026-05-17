using System.Diagnostics.Metrics;

namespace WolverinePoc.Api.Infrastructure;

public sealed class AppMetrics
{
    private readonly Lock _sync = new();
    private readonly Meter _meter = new("WolverinePoc.Api");
    private readonly Counter<long> _requests;
    private readonly Counter<long> _failedMessages;
    private readonly Histogram<double> _orderCreateLatency;
    private readonly Histogram<double> _handlerDuration;

    private long _requestCount;
    private long _failedMessageCount;
    private DateTime _startedUtc = DateTime.UtcNow;
    private double _lastOrderCreateLatencyMs;
    private double _lastHandlerDurationMs;

    public AppMetrics()
    {
        _requests = _meter.CreateCounter<long>("requests.total");
        _failedMessages = _meter.CreateCounter<long>("messages.failed.total");
        _orderCreateLatency = _meter.CreateHistogram<double>("orders.create.latency.ms");
        _handlerDuration = _meter.CreateHistogram<double>("handlers.duration.ms");
    }

    public void RecordRequestStarted()
    {
        Interlocked.Increment(ref _requestCount);
        _requests.Add(1);
    }

    public void RecordOrderCreateLatency(TimeSpan duration)
    {
        _lastOrderCreateLatencyMs = duration.TotalMilliseconds;
        _orderCreateLatency.Record(duration.TotalMilliseconds);
    }

    public void RecordHandlerDuration(TimeSpan duration)
    {
        _lastHandlerDurationMs = duration.TotalMilliseconds;
        _handlerDuration.Record(duration.TotalMilliseconds);
    }

    public void RecordFailedMessage()
    {
        Interlocked.Increment(ref _failedMessageCount);
        _failedMessages.Add(1);
    }

    public object Snapshot()
    {
        lock (_sync)
        {
            var uptime = DateTime.UtcNow - _startedUtc;
            var requests = Interlocked.Read(ref _requestCount);

            return new
            {
                requestsTotal = requests,
                requestsPerSecond = uptime.TotalSeconds <= 0 ? 0 : Math.Round(requests / uptime.TotalSeconds, 2),
                lastOrderCreateLatencyMs = Math.Round(_lastOrderCreateLatencyMs, 2),
                lastHandlerDurationMs = Math.Round(_lastHandlerDurationMs, 2),
                retriesCount = "tracked by Wolverine error handling logs and durable message store",
                failedMessages = Interlocked.Read(ref _failedMessageCount),
                uptimeSeconds = Math.Round(uptime.TotalSeconds, 2)
            };
        }
    }
}
