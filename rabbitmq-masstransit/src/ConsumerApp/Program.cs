using MassTransit;
using Messaging.Lib;
using Messaging.Lib.Consumers;
using Messaging.Lib.Entities;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SqlDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration["SqlDbContext"], options =>
    {
        options.MigrationsAssembly(typeof(SqlDbContext).Assembly.FullName);
        options.MigrationsHistoryTable($"__{nameof(SqlDbContext)}");
    });
}, ServiceLifetime.Scoped);

var settings =  builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.AddEntityFrameworkOutbox<SqlDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(1);

        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(settings.Host, c =>
        {
            c.Username(settings.Username);
            c.Password(settings.Password);
        });

        cfg.ReceiveEndpoint("order-created-event", e =>
        {
            e.UseEntityFrameworkOutbox<SqlDbContext>(context);
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });
    });
});


var app = builder.Build();

app.MapGet("/", () => "Hello world from consumer app");

app.Run();
