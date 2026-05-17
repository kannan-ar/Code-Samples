using Microsoft.EntityFrameworkCore;
using WolverinePoc.Api.Infrastructure;

namespace WolverinePoc.Api.Features.Orders.GetOrder;

public static class GetOrderHandler
{
    public static Task<OrderDetails?> Handle(
        GetOrderQuery query,
        OrdersDbContext dbContext,
        CancellationToken cancellationToken)
    {
        return dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Id == query.OrderId)
            .Select(order => new OrderDetails(
                order.Id,
                order.CustomerName,
                order.ProductCode,
                order.Quantity,
                order.UnitPrice,
                order.TotalAmount,
                order.Status.ToString(),
                order.CreatedUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
