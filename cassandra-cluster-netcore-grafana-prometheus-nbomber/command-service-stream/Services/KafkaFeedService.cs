using Confluent.Kafka;
using Grpc.Core;
using Polly;
using Polly.Retry;
using SharedLibraries.Schemas;
using System.Text.Json;
using System.Threading.Channels;

namespace CommandServiceStreamApi.Services
{
    public class KafkaFeedService : FeedService.FeedServiceBase
    {
        private readonly ChannelWriter<OrderCreated> _writer;
        private readonly IProducer<string, string> _kafkaProducer;
        private readonly ILogger<KafkaFeedService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public KafkaFeedService(
            ChannelWriter<OrderCreated> writer,
            IProducer<string, string> kafkaProducer,
            ILogger<KafkaFeedService> logger)
        {
            _writer = writer;
            _kafkaProducer = kafkaProducer;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<ProduceException<string, string>>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt)),
                    onRetry: (exception, timespan, retryCount, context) =>
                    {
                        _logger.LogWarning($"Retry {retryCount} after {timespan.TotalMilliseconds}ms due to {exception.Message}");
                    });
        }

        public override async Task<Ack> StreamEvents(IAsyncStreamReader<OrderCreated> requestStream, ServerCallContext context)
        {
            await foreach (var orderEvent in requestStream.ReadAllAsync(context.CancellationToken))
            {
                if (await _writer.WaitToWriteAsync(context.CancellationToken))
                {
                    await _writer.WriteAsync(orderEvent, context.CancellationToken);
                }
                else
                {
                    _logger.LogWarning("Channel full, writing directly to Kafka with retry");

                    var key = orderEvent.OrderId;
                    var value = JsonSerializer.Serialize(orderEvent);

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var result = await _kafkaProducer.ProduceAsync("order-events", new Message<string, string>
                        {
                            Key = key,
                            Value = value
                        });

                        _logger.LogInformation($"Fallback Kafka write succeeded: {result.TopicPartitionOffset}");
                    });
                }
            }

            return new Ack { Message = "Stream completed" };
        }
    }
}
