using Serilog.Context;
using Wolverine;
using Wolverine.Attributes;
using WolverinePoc.Api.Features.Messaging;
using WolverinePoc.Api.Features.Orders;
using WolverinePoc.Api.Infrastructure;
using WolverinePoc.Api.Shared;

namespace WolverinePoc.Api.Features.Orders.CreateOrder;

public static class CreateOrderHandler
{
    [Transactional]
    public static async Task<CreateOrderResponse> Handle(
        CreateOrderCommand command,
        OrdersDbContext dbContext,
        IMessageBus bus,
        AppMetrics metrics,
        TimeProvider timeProvider,
        ILogger<CreateOrderCommand> logger,
        CancellationToken cancellationToken)
    {
        var started = timeProvider.GetTimestamp();

        Validate(command);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = command.CustomerName!.Trim(),
            ProductCode = command.ProductCode!.Trim(),
            Quantity = command.Quantity,
            UnitPrice = command.UnitPrice,
            TotalAmount = command.Quantity * command.UnitPrice,
            Status = OrderStatus.Pending,
            CreatedUtc = timeProvider.GetUtcNow().UtcDateTime
        };

        using (LogContext.PushProperty("OrderId", order.Id))
        {
            dbContext.Orders.Add(order);
            var messageId = Guid.NewGuid();

            await bus.PublishAsync(new OrderCreated(
                messageId,
                order.Id,
                order.CustomerName,
                order.ProductCode,
                order.Quantity,
                order.UnitPrice,
                order.TotalAmount,
                order.CreatedUtc));

            metrics.RecordHandlerDuration(timeProvider.GetElapsedTime(started));

            using (LogContext.PushProperty("MessageId", messageId))
            {
                logger.LogInformation(
                    "Order created and OrderCreated event queued in transactional outbox for Kafka publish");
            }

            return new CreateOrderResponse(order.Id, order.Status.ToString());
        }
    }

    private static void Validate(CreateOrderCommand command)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(command.CustomerName))
        {
            errors[nameof(command.CustomerName)] = ["customerName is required."];
        }

        if (string.IsNullOrWhiteSpace(command.ProductCode))
        {
            errors[nameof(command.ProductCode)] = ["productCode is required."];
        }

        if (command.Quantity <= 0)
        {
            errors[nameof(command.Quantity)] = ["quantity must be greater than zero."];
        }

        if (command.UnitPrice <= 0)
        {
            errors[nameof(command.UnitPrice)] = ["unitPrice must be greater than zero."];
        }

        if (errors.Count > 0)
        {
            throw new ValidationException(errors);
        }
    }
}
