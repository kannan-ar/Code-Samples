using ApiGateway.Modals;
using ApiGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace ApiGateway.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly IQueueService queueService;
        private readonly QueueNameSettings queueNameSettings;

        public EntityController(
            IQueueService queueService,
            IOptions<QueueNameSettings> queueNameSettings)
        {
            this.queueService = queueService;
            this.queueNameSettings = queueNameSettings.Value;
        }

        [HttpPost]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Post(Entity entity)
        {
            await queueService.AddMessageAsync(queueNameSettings.ProcessEntityQueueName, JsonSerializer.Serialize<Entity>(entity));
            return Accepted(entity);
        }
    }
}
