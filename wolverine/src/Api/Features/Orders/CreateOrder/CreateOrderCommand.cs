namespace WolverinePoc.Api.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    string? CustomerName,
    string? ProductCode,
    int Quantity,
    decimal UnitPrice);
