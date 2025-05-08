using Asp.Versioning;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace CommandServiceApi.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class OrderController(ITopicProducer<OrderCreated> producer) : ControllerBase
    {
        private readonly ITopicProducer<OrderCreated> _producer = producer;

        [HttpPost]
        [ProducesResponseType(201)]
        public async Task<IActionResult> Create(OrderCreated orderCreated)
        {
            try
            {
                await _producer.Produce(orderCreated);

            }
            catch (Exception ex)
            {

                throw;
            }

            return Created();
        }
    }
}
