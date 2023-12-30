using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoStreamer.API.Authorization;

namespace VideoStreamer.API.Controllers.Contribute
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(StreamerPolicies.ContributorPolicy)]
    public class VideoController : ControllerBase
    {
        [HttpPost]
        public IActionResult Post()
        {
            return Accepted();
        }
    }
}
