using Serilog.Context;
using Wolverine.Attributes;
using WolverinePoc.Api.Features.Orders;
using WolverinePoc.Api.Infrastructure;

namespace WolverinePoc.Api.Features.Messaging;

public static class OrderCreatedHandler
{
    [Transactional]
    public static async Task Handle(
        OrderCreated message,
        OrdersDbContext dbContext,
        AppMetrics metrics,
        ILogger<OrderCreated> logger,
        CancellationToken cancellationToken)
    {
        using (LogContext.PushProperty("OrderId", message.OrderId))
        using (LogContext.PushProperty("MessageId", message.MessageId))
        {
            try
            {
                var order = await dbContext.Orders.FindAsync([message.OrderId], cancellationToken);

                if (order is null)
                {
                    throw new InvalidOperationException($"Order {message.OrderId} was not found.");
                }

                if (order.Status == OrderStatus.Confirmed)
                {
                    logger.LogInformation("Duplicate OrderCreated message ignored for already confirmed order");
                    return;
                }

                order.Status = OrderStatus.Confirmed;
                logger.LogInformation("Order confirmed by Kafka consumer");
            }
            catch
            {
                metrics.RecordFailedMessage();
                logger.LogError("OrderCreated consumer failed");
                throw;
            }
        }
    }
}
