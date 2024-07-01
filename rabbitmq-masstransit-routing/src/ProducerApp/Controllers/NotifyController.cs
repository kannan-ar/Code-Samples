using Messaging.Lib;
using Messaging.Lib.Messages;
using Microsoft.AspNetCore.Mvc;

namespace ProducerApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly IPublishManager _publishManager;

        public NotifyController(IPublishManager publishManager)
        {
            _publishManager = publishManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotifiedRegion model)
        {
            await _publishManager.Publish(model);

            return Accepted();
        }
    }
}
