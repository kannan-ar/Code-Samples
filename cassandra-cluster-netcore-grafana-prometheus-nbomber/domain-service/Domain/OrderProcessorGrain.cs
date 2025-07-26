using Cassandra;
using DomainService.Contracts;
using System.Threading.Channels;
using System.Diagnostics.Metrics;

namespace DomainService.Domain;

public class OrderProcessorGrain(ILogger<OrderProcessorGrain> logger) : Grain, IOrderProcessorGrain
{
    private readonly ILogger<OrderProcessorGrain> _logger = logger;
    private Channel<string> _channel;
    private ICluster _cassandraCluster;
    private ISession _session;
    private PreparedStatement _preparedStatement;

    private List<string> _batchBuffer;
    private readonly int _batchSize = 50;
    private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(5);

    private GrainCancellationTokenSource _cts;

    private static long _activationCount = 0;
    private static readonly Meter _meter = new("DomainService.Domain.OrderProcessorGrain", "1.0.0");

    private static readonly ObservableGauge<long> _activationGauge =
        _meter.CreateObservableGauge(
            "order_processor_grain_activations",
            () => new Measurement<long>(Interlocked.Read(ref _activationCount)),
            unit: "count",
            description: "Current number of active IOrderProcessorGrain grains");

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _cts = new GrainCancellationTokenSource();

        Interlocked.Increment(ref _activationCount);

        // Connect to Cassandra
        _session = await ConnectToCassandraAsync();
        _preparedStatement = await _session.PrepareAsync("INSERT INTO mykeyspace.events (id, payload) VALUES (?, ?)");

        // Init channel and buffer
        _channel = Channel.CreateBounded<string>(new BoundedChannelOptions(1000)
        {
            SingleWriter = false,
            SingleReader = true,
            FullMode = BoundedChannelFullMode.Wait
        });

        _batchBuffer = [];

        // Start background loops
        _ = ProcessChannelAsync(_cts.Token.CancellationToken);
        _ = RunBatchLoopAsync(_cts.Token.CancellationToken, _flushInterval);

        await base.OnActivateAsync(cancellationToken);
    }

    public override Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        Interlocked.Decrement(ref _activationCount);

        _cts?.Cancel();
        return Task.CompletedTask;
    }

    public Task Process(string message)
    {
        return _channel.Writer.WriteAsync(message).AsTask();
    }

    private async Task ProcessChannelAsync(CancellationToken token)
    {
        try
        {
            await foreach (var message in _channel.Reader.ReadAllAsync(token))
            {
                _batchBuffer.Add(message);

                if (_batchBuffer.Count >= _batchSize)
                {
                    await FlushBatchAsync();
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Channel reader cancelled.");
        }
    }

    private async Task RunBatchLoopAsync(CancellationToken token, TimeSpan interval)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(interval, token);
                await FlushBatchAsync();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch timer loop");
            }
        }
    }

    private async Task FlushBatchAsync()
    {
        if (_batchBuffer.Count == 0)
            return;

        var batch = new BatchStatement();

        foreach (var msg in _batchBuffer)
        {
            var id = Guid.NewGuid(); // Use Kafka offset if available
            var bound = _preparedStatement.Bind(id, msg);
            batch.Add(bound);
        }

        try
        {
            await _session.ExecuteAsync(batch);
            _logger.LogInformation("Flushed batch of {Count} messages to Cassandra", _batchBuffer.Count);
            _batchBuffer.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write batch to Cassandra");
            // Optionally retry or move to dead-letter logic here
        }
    }

    private Task<ISession> ConnectToCassandraAsync()
    {
        // You can pool this at the app level for better reuse
        _cassandraCluster = Cluster.Builder()
            .AddContactPoint("localhost")
            .Build();

        return _cassandraCluster.ConnectAsync("mykeyspace");
    }
}