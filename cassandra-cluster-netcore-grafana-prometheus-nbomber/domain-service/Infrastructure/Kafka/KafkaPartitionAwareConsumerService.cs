using Confluent.Kafka;
using DomainService.Contracts;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace DomainService.Infrastructure.Kafka;

public class KafkaPartitionAwareConsumerService : BackgroundService
{
    private readonly ILogger<KafkaPartitionAwareConsumerService> _logger;
    private readonly IGrainFactory _grainFactory;
    private readonly string _topic;
    private readonly ConsumerConfig _baseConfig;
    private IConsumer<string, string> _consumer;

    private readonly ConcurrentDictionary<TopicPartition, CancellationTokenSource> _partitionTokens = new();
    private readonly ConcurrentDictionary<TopicPartition, Task> _partitionTasks = new();

    public KafkaPartitionAwareConsumerService(
        ILogger<KafkaPartitionAwareConsumerService> logger,
        IGrainFactory grainFactory,
        IConfiguration config)
    {
        _logger = logger;
        _grainFactory = grainFactory;
        _topic = config["Kafka:TopicName"] ?? string.Empty;

        _baseConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = config["Kafka:GroupName"],
            EnableAutoCommit = false,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            AllowAutoCreateTopics = true
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer = new ConsumerBuilder<string, string>(_baseConfig)
            .SetPartitionsAssignedHandler(OnPartitionsAssigned)
            .SetPartitionsRevokedHandler(OnPartitionsRevoked)
            .Build();

        _consumer.Subscribe(_topic);

        _logger.LogInformation("KafkaConsumerService started and subscribed to topic {Topic}", _topic);

        // Start polling to trigger assignment
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _consumer.Consume(TimeSpan.FromMilliseconds(100));
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Polling error");
            }
        }
    }

    private List<TopicPartitionOffset> OnPartitionsAssigned(IConsumer<string, string> consumer, List<TopicPartition> partitions)
    {
        _logger.LogInformation("Partitions assigned: {Partitions}", string.Join(", ", partitions.Select(p => p.Partition.Value)));

        foreach (var partition in partitions)
        {
            if (_partitionTasks.ContainsKey(partition))
                continue;

            var cts = new CancellationTokenSource();
            var task = Task.Run(() => StartPartitionConsumer(partition, cts.Token));
            _partitionTokens[partition] = cts;
            _partitionTasks[partition] = task;
        }

        // Start from last committed offset or earliest
        return partitions.Select(p => new TopicPartitionOffset(p, Offset.Unset)).ToList();
    }

    private void OnPartitionsRevoked(IConsumer<string, string> consumer, List<TopicPartitionOffset> partitions)
    {
        _logger.LogWarning("Partitions revoked: {Partitions}", string.Join(", ", partitions.Select(p => p.Partition.Value)));

        foreach (var tpo in partitions)
        {
            var tp = tpo.TopicPartition;

            if (_partitionTokens.TryRemove(tp, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
            }

            if (_partitionTasks.TryRemove(tp, out var task))
            {
                _ = task.ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        _logger.LogError(t.Exception, "Partition consumer task faulted after removal.");
                }, TaskContinuationOptions.OnlyOnFaulted);
            }
        }
    }

    private async Task StartPartitionConsumer(TopicPartition partition, CancellationToken cancellationToken)
    {
        var config = new ConsumerConfig(_baseConfig)
        {
            ClientId = $"consumer-{partition.Partition.Value}-{Guid.NewGuid()}",
            GroupId = _baseConfig.GroupId // Same groupId to participate in rebalance
        };

        using var partitionConsumer = new ConsumerBuilder<string, string>(config).Build();
        partitionConsumer.Assign(partition);

        _logger.LogInformation("Started consuming partition {Partition}", partition.Partition.Value);

        var grain = _grainFactory.GetGrain<IOrderProcessorGrain>(partition.Partition.Value.ToString());

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = partitionConsumer.Consume(cancellationToken);
                if (result?.IsPartitionEOF != false)
                    continue;

                await grain.Process(result.Message.Value);

                partitionConsumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                _logger.LogError(ex, "Kafka error on partition {Partition}", partition.Partition.Value);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer cancelled for partition {Partition}", partition.Partition.Value);
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in partition {Partition}", partition.Partition.Value);
                await Task.Delay(1000, cancellationToken); // Brief pause on error
            }
        }

        _logger.LogInformation("Stopped consuming partition {Partition}", partition.Partition.Value);
    }

    public override void Dispose()
    {
        base.Dispose();
        _consumer?.Close();
        _consumer?.Dispose();
    }
}
