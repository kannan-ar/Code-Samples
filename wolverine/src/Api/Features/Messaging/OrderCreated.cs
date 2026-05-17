namespace WolverinePoc.Api.Features.Messaging;

public sealed record OrderCreated(
    Guid MessageId,
    Guid OrderId,
    string CustomerName,
    string ProductCode,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    DateTime CreatedUtc);
