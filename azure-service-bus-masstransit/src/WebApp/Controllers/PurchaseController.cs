using AutoMapper;
using Messaging.Lib;
using Messaging.Lib.QueueMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IQueueManager _queueManager;
        private readonly IMapper _mapper;
        private readonly QueueSettings _queueSettings;

        public PurchaseController(IQueueManager queueManager, IMapper mapper, IOptions<QueueSettings> queueSettings)
        {
            _queueManager = queueManager;
            _mapper = mapper;
            _queueSettings = queueSettings.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PurchaseOrder purchase)
        {
            await _queueManager.SendMessage(_mapper.Map<PurchaseCreated>(purchase), _queueSettings.PurchaseQueueName);
            return Accepted();
        }
    }
}
