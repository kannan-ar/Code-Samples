using ConsumerApp;
using MassTransit;
using MassTransit.Transports.Fabric;
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
    x.AddConsumer<RegularCustomerRegisteredConsumer>();
    x.AddConsumer<PremiumCustomerRegisteredConsumer>();

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

        cfg.ConfigureConsumerEndPoint<OrderCreatedConsumer>(context, "order-created-event");

        cfg.ConfigureConsumerEndPoint<RegularCustomerRegisteredConsumer>(context, "regular-customer-registered-event", "customerregistered", "regular", "direct");
        cfg.ConfigureConsumerEndPoint<PremiumCustomerRegisteredConsumer>(context, "premium-customer-registered-event", "customerregistered", "premium", "direct");
    });
});

var app = builder.Build();

app.MapGet("/", () => "Hello world from consumer app");

app.Run();
