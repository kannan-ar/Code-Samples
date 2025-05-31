
using Confluent.Kafka;
using DomainService.Contracts;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;

namespace DomainService.Infrastructure.Kafka;

public class KafkaPartitionAwareConsumerService : BackgroundService
{
    private readonly ILogger<KafkaPartitionAwareConsumerService> _logger;
    private readonly IGrainFactory _grainFactory;
    private readonly ConsumerConfig _consumerConfig;
    private readonly string _topic;

    private readonly ConcurrentDictionary<TopicPartition, Task> _partitionTasks = new();

    public KafkaPartitionAwareConsumerService(
        ILogger<KafkaPartitionAwareConsumerService> logger,
        IGrainFactory grainFactory,
        IConfiguration config)
    {
        _logger = logger;
        _grainFactory = grainFactory;
        _topic = config["Kafka:TopicName"] ?? string.Empty;

        _consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "order-consumer-group",
            EnableAutoCommit = false,
            EnablePartitionEof = false,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() =>
        {
            using var consumer = new ConsumerBuilder<string, string>(_consumerConfig)
            .SetPartitionsAssignedHandler((c, partitions) =>
            {
                foreach (var partition in partitions)
                {
                    if (!_partitionTasks.ContainsKey(partition))
                    {
                        var task = StartPartitionConsumer(partition, stoppingToken);
                        _partitionTasks.TryAdd(partition, task);
                    }
                }

                return partitions.Select(p => new TopicPartitionOffset(p, Offset.Unset));
            })
            .SetPartitionsRevokedHandler((c, partitions) =>
            {
                foreach (var partition in partitions)
                {
                    _partitionTasks.TryRemove(partition.TopicPartition, out _);
                }
            })
            .Build();

            consumer.Subscribe(_topic);
        }, stoppingToken);
    }

    private Task StartPartitionConsumer(TopicPartition partition, CancellationToken token)
    {
        return Task.Run(async () =>
        {
            var config = new ConsumerConfig(_consumerConfig)
            {
                GroupId = $"order-processor-group-{partition.Partition.Value}" // Partition-aware group ID
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Assign(partition);

            var channel = Channel.CreateUnbounded<ConsumeResult<string, string>>();
            _ = StartDispatcher(channel.Reader, _grainFactory, consumer, token);

            while (!token.IsCancellationRequested)
            {
                try
                {
                    var msg = consumer.Consume(token);
                    await channel.Writer.WriteAsync(msg, token);
                }
                catch (ConsumeException ex)
                {
                    Console.WriteLine($"Consume error: {ex.Error.Reason}");
                }
            }
        }, token);
    }

    private static async Task StartDispatcher(
        ChannelReader<ConsumeResult<string, string>> reader,
        IGrainFactory grainFactory,
        IConsumer<string, string> consumer,
        CancellationToken token)
    {
        await foreach (var msg in reader.ReadAllAsync(token))
        {
            try
            {
                var grain = grainFactory.GetGrain<IOrderProcessorGrain>(msg.Message.Key);
                await grain.Process(msg.Message.Value);
                consumer.Commit(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Processing error: {ex.Message}");
                // Optional: Send to dead-letter topic
            }
        }
    }
}
