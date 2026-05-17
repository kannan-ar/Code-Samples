namespace WolverinePoc.Api.Features.Orders.CreateOrder;

public sealed record CreateOrderRequest(
    string? CustomerName,
    string? ProductCode,
    int Quantity,
    decimal UnitPrice)
{
    public CreateOrderCommand ToCommand() => new(CustomerName, ProductCode, Quantity, UnitPrice);
}
