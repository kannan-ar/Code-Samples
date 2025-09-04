using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Context.Propagation;
using SharedLibraries.Schemas;
using System.Diagnostics;

namespace CommandServiceApi.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class OrderController(ITopicProducer<string, OrderCreated> producer) : ControllerBase
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create(OrderCreated orderCreated)
    {
        await producer.Produce(orderCreated.OrderId, orderCreated, Pipe.Execute<KafkaSendContext<string, OrderCreated>>(context =>
        {
            if (Activity.Current is { } activity)
            {
                context.Headers.Set("traceparent", activity.Id);
                context.Headers.Set("trace-id", activity.TraceId.ToString());
                context.Headers.Set("span-id", activity.SpanId.ToString());
                context.Headers.Set("trace-flags", ((int)activity.ActivityTraceFlags).ToString());
            }
        }));

        return Created();
    }
}

