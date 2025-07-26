using CommandServiceStreamApi.Services;
using SharedLibraries.Schemas;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var channel = Channel.CreateBounded<OrderCreated>(new BoundedChannelOptions(5000)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = false,
    SingleReader = true
});

builder.Services.AddSingleton(channel);
builder.Services.AddSingleton(channel.Writer);
builder.Services.AddSingleton(channel.Reader);

builder.Services.AddHostedService<KafkaWriterService>();
builder.Services.AddSingleton<KafkaFeedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGrpcService<KafkaFeedService>();
app.MapGet("/", () => "Use a gRPC client to stream OrderEvents.");

app.Run();

