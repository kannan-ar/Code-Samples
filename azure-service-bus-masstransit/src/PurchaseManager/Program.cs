using MassTransit;
using Messaging.Lib.Consumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PurchaseCreatedConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration["ArServiceBus"]);

        cfg.ReceiveEndpoint(builder.Configuration["QueueSettings:PurchaseQueueName"], e =>
        {
            e.Consumer<PurchaseCreatedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
