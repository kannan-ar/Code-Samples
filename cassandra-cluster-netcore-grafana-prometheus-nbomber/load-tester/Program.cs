using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using NBomber.CSharp;
using SharedLibraries.Schemas;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var scenario = Scenario.Create("grpc_streaming_test", async context =>
{
    // Simulate reconnection per scenario iteration
    try
    {
        var grpcUrl = config["GRPC_SERVER_URL"] ?? throw new ArgumentNullException("GRPC_SERVER_URL not set in configuration");

        using var channel = GrpcChannel.ForAddress(grpcUrl);
        var client = new FeedService.FeedServiceClient(channel);

        using var call = client.StreamEvents();

        var rand = new Random();
        int messagesToSend = rand.Next(5, 15); // Send 5–15 messages per stream

        for (int i = 0; i < messagesToSend; i++)
        {
            var order = new OrderCreated
            {
                OrderId = Guid.NewGuid().ToString(),
                Amount = rand.NextDouble() * 1000,
            };

            try
            {
                await Step.Run("send_order", context, async () =>
                {
                    return await SendWithRetryAsync(order, call.RequestStream);
                });

            }
            catch (Exception ex)
            {
                return Response.Fail(message: $"Failed to send order {order.OrderId}: {ex.Message}");
            }
        }

        return Response.Ok(payload: messagesToSend); // Report how many messages were sent
    }
    catch (Exception ex)
    {
        return Response.Fail(message: $"gRPC stream error: {ex.Message}");
    }
})
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(30)))
//Ramp-Up → Steady State → Cooldown
.WithLoadSimulations(
    Simulation.RampingConstant(copies: 10, during: TimeSpan.FromSeconds(10)), // Warm-up
    Simulation.KeepConstant(copies: 50, during: TimeSpan.FromSeconds(60)), // Steady state
    Simulation.RampingConstant(copies: 0, during: TimeSpan.FromSeconds(10)))      // Cooldown
//Traffic Spikes
.WithLoadSimulations(
    Simulation.RampingInject(rate: 5, interval: TimeSpan.FromSeconds(10), during: TimeSpan.FromSeconds(10)),
    Simulation.RampingInject(rate: 50, interval: TimeSpan.FromSeconds(5), during: TimeSpan.FromSeconds(5)),  // Spike
    Simulation.RampingInject(rate: 5, interval: TimeSpan.FromSeconds(10), during: TimeSpan.FromSeconds(10)))
//Cyclic Load with Pause Between Waves
.WithLoadSimulations(
    Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(20)),
    Simulation.Pause(TimeSpan.FromSeconds(5)),
    Simulation.KeepConstant(copies: 40, during: TimeSpan.FromSeconds(20)),
    Simulation.Pause(TimeSpan.FromSeconds(5)),
    Simulation.KeepConstant(copies: 10, during: TimeSpan.FromSeconds(20)));

NBomberRunner.RegisterScenarios(scenario).Run();

static async Task<NBomber.Contracts.Response<object>> SendWithRetryAsync(OrderCreated order, IClientStreamWriter<OrderCreated> streamWriter, int maxRetries = 3)
{
    int attempt = 0;

    while (attempt < maxRetries)
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            await streamWriter.WriteAsync(order, cts.Token);
            return Response.Ok();
        }
        catch (Exception ex)
        {
            attempt++;
            if (attempt >= maxRetries)
                return Response.Fail(message: $"Failed after {maxRetries} retries: {ex.Message}");

            await Task.Delay(TimeSpan.FromMilliseconds(200 * attempt)); // Exponential backoff
        }
    }

    return Response.Fail(message: "Unexpected failure"); // Fallback safeguard
}