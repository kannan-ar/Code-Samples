using Asp.Versioning;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SharedLibraries.Schemas;

namespace CommandServiceApi.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class OrderController(ITopicProducer<string, OrderCreated> producer) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(201)]
    public async Task<IActionResult> Create(OrderCreated orderCreated)
    {
        await producer.Produce(orderCreated.OrderId, orderCreated);

        return Created();
    }
}

