namespace WolverinePoc.Api.Features.Orders.GetOrder;

public sealed record OrderDetails(
    Guid OrderId,
    string CustomerName,
    string ProductCode,
    int Quantity,
    decimal UnitPrice,
    decimal TotalAmount,
    string Status,
    DateTime CreatedUtc);
