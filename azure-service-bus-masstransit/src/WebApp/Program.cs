using MassTransit;
using Messaging.Lib;
using Messaging.Lib.QueueMessages;
using WebApp.Profiles;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddAutoMapper(typeof(PurchaseProfile));

builder.Services.AddScoped<IQueueManager, QueueManager>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection("QueueSettings"));

builder.Services.AddMassTransit(x =>
{
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(builder.Configuration["ArServiceBus"]);
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
