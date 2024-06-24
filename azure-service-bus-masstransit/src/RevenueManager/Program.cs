using MassTransit;
using Messaging.Lib.Consumers;
using Messaging.Lib.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PurchaseCompletedConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration["ArServiceBus"]);

        cfg.SubscriptionEndpoint<PurchaseCompleted>("revenue-subscriber", e =>
        {
            e.ConfigureConsumer<PurchaseCompletedConsumer>(context);
        });
    });
});

var app = builder.Build();

app.Run();
