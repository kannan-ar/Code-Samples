using MassTransit;
using ShoppingApp.Api.Contracts;

namespace ShoppingApp.Api.Consumers
{
    public class OrderSubmittedConsumer : IConsumer<OrderSubmitted>
    {
        public Task Consume(ConsumeContext<OrderSubmitted> context)
        {
            var message = context.Message;
            Console.WriteLine($"📦 Order received: {message.OrderId} - {message.ProductName} x {message.Quantity}");

            // You can call services, log, write to DB here
            return Task.CompletedTask;
        }
    }
