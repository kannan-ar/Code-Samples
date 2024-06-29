using Messaging.Lib;
using Messaging.Lib.Messages;
using Microsoft.AspNetCore.Mvc;

namespace ProducerApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IPublishManager _publishManager;

        public OrderController(IPublishManager publishManager)
        {
            _publishManager = publishManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            await _publishManager.Publish(new OrderCreated
            {
                OrderId = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
            });

            return Accepted();
        }
    }
}
