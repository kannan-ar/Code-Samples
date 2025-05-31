using System.IO.Pipes;
using System.Text;

Console.WriteLine("Starting NamedPipeSender...");

int counter = 1;

while (true)
{
    using var pipeClient = new NamedPipeClientStream(".", "mypipe", PipeDirection.Out);
    try
    {
        await pipeClient.ConnectAsync(1000); // 1 second timeout

        string message = $"Hello #{counter++} at {DateTime.Now}";
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await pipeClient.WriteAsync(buffer, 0, buffer.Length);
        Console.WriteLine($"Sent: {message}");
    }
    catch (TimeoutException)
    {
        Console.WriteLine("Could not connect to pipe server. Retrying...");
    }

    await Task.Delay(2000); // send every 2 seconds
}