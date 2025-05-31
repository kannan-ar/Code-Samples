using System.IO.Pipes;
using System.Text;

namespace NamedPipeListener
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () =>
            {
                _logger.LogInformation("NamedPipeListener started. Waiting for messages...");

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var pipeServer = new NamedPipeServerStream("mypipe", PipeDirection.In);
                    await pipeServer.WaitForConnectionAsync(stoppingToken);

                    using var reader = new StreamReader(pipeServer, Encoding.UTF8);
                    var message = await reader.ReadToEndAsync();
                    _logger.LogInformation("Received: {Message}", message);
                }
            });
        }
    }
}
