using MassTransit;
using Messaging.Lib;
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
        cfg.Host("amqp://rabbitmq:5672", c =>
        {
            c.Username("guest");
            c.Password("guest");
        });

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
