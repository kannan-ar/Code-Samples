using MediaServices.MediaViewer.Models;
using MediaServices.MediaViewer.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace MediaServices.MediaViewer.Controllers
{
    [Route("metadata")]
    [ApiController]
    public class GetMetadataController : ControllerBase
    {
        private readonly SiteSettings _siteSettings;

        public GetMetadataController(IOptions<SiteSettings> siteSettings)
        {
            _siteSettings = siteSettings.Value;
        }

        [HttpGet("{containerName}")]
        public ActionResult<string> Video(MediaType mediaType, string containerName)
        {
            return Ok($"{_siteSettings.BaseUrl}/{containerName}/{containerName}.mpd");
        }
    }
}
