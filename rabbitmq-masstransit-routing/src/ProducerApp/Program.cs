using MassTransit;
using MassTransit.Transports.Fabric;
using Messaging.Lib;
using Messaging.Lib.Entities;
using Messaging.Lib.Messages;
using Microsoft.EntityFrameworkCore;
using ProducerApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPublishManager, PublishManager>();

builder.Services.AddDbContext<SqlDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration["SqlDbContext"], options =>
    {
        options.MigrationsAssembly(typeof(SqlDbContext).Assembly.FullName);
        options.MigrationsHistoryTable($"__{nameof(SqlDbContext)}");
    });
}, ServiceLifetime.Scoped);

builder.Services.InitDatabase();

var settings = builder.Configuration.GetSection("RabbitMqSettings").Get<RabbitMqSettings>();

builder.Services.AddMassTransit(x =>
{
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

        cfg.ConfigExchange<CustomerRegistered>(x => x.Message.Type, "customerregistered", "direct");
        cfg.ConfigExchange<NotifiedRegion>(x => x.Message.Region, "notifyregion", "topic");

        cfg.ConfigureEndpoints(context);
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
