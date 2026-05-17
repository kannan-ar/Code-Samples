namespace WolverinePoc.Api.Features.Orders;

public sealed class Order
{
    public Guid Id { get; set; }

    public required string CustomerName { get; set; }

    public required string ProductCode { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalAmount { get; set; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedUtc { get; set; }
}
