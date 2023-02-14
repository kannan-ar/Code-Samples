using ApiGateway.Modals;
using ApiGateway.Services;
using ApiGateway.Services.Implementations;
using Azure.Storage.Queues;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("Queues.json");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<QueueNameSettings>(builder.Configuration.GetSection("QueueNameSettings"));

var connectionString = builder.Configuration.GetConnectionString("QueueStorage");

builder.Services.AddAzureClients(options =>
{
    options.AddQueueServiceClient(connectionString)
            .ConfigureOptions(o => o.MessageEncoding = QueueMessageEncoding.Base64);
});

builder.Services.AddSingleton<IQueueService, QueueService>();

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
