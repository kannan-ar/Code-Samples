using MassTransit;
using ShoppingApp.Api.Consumers;
using ShoppingApp.Api.Contracts;
using ShoppingApp.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderSubmittedConsumer>();

    x.UsingInMemory();
  
    x.AddRider(rider =>
    {
        rider.AddConsumer<OrderSubmittedConsumer>();

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");

            k.TopicEndpoint<OrderSubmitted>(
                topicName: "orders",
                groupId: "order-service-group",
                configure: e =>
                {
                    e.ConfigureConsumer<OrderSubmittedConsumer>(context);
                });
        });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<CassandraSettings>(builder.Configuration.GetSection("CassandraSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
