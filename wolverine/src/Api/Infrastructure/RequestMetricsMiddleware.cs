namespace WolverinePoc.Api.Infrastructure;

public sealed class RequestMetricsMiddleware(RequestDelegate next, AppMetrics metrics, TimeProvider timeProvider)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var started = timeProvider.GetTimestamp();
        metrics.RecordRequestStarted();

        try
        {
            await next(context);
        }
        finally
        {
            if (context.Request.Path == "/orders" && context.Request.Method == HttpMethods.Post)
            {
                metrics.RecordOrderCreateLatency(timeProvider.GetElapsedTime(started));
            }
        }
    }
}
