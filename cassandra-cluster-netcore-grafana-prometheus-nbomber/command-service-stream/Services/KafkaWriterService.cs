using Confluent.Kafka;
using SharedLibraries.Schemas;
using System.Threading.Channels;

namespace CommandServiceStreamApi.Services
{
    public class KafkaWriterService(Channel<OrderCreated> channel, IProducer<string, string> producer) : BackgroundService
    {
        private readonly Channel<OrderCreated> _channel = channel;
        private readonly IProducer<string, string> _producer = producer;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var buffer = new List<OrderCreated>();
            var batchSize = 100;
            var flushInterval = TimeSpan.FromMilliseconds(200);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var delayTask = Task.Delay(flushInterval, stoppingToken);
                    while (buffer.Count < batchSize && _channel.Reader.TryRead(out var message))
                        buffer.Add(message);

                    if (buffer.Count > 0)
                    {
                        foreach (var msg in buffer)
                            await _producer.ProduceAsync("order-events", new Message<string, string> { Key = msg.OrderId, Value = msg.ToString() }, stoppingToken);

                        buffer.Clear();
                    }

                    await delayTask;
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex) { Console.WriteLine($"Kafka error: {ex.Message}"); }
            }
        }
    }
}
