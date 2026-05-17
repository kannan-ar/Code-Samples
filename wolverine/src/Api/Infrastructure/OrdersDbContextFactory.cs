using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WolverinePoc.Api.Infrastructure;

public sealed class OrdersDbContextFactory : IDesignTimeDbContextFactory<OrdersDbContext>
{
    public OrdersDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrdersDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=wolverine_poc;Username=wolverine;Password=wolverine")
            .Options;

        return new OrdersDbContext(options);
    }
}
