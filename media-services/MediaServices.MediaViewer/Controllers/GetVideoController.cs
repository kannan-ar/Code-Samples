using MediaServices.MediaViewer.Services;
using Microsoft.AspNetCore.Mvc;

namespace MediaServices.MediaViewer.Controllers
{
    [ApiController]
    public class GetVideoController : ControllerBase
    {
        private readonly IBlobManager _blobManager;

        public GetVideoController(IBlobManager blobManager)
        {
            _blobManager = blobManager;
        }

        [HttpGet("Video/{containerName}/{fileName}")]
        public async Task<IActionResult> Video(string containerName, string fileName)
        {
            try
            {
                var stream = await _blobManager.GetBlob(containerName, fileName);
                return File(stream, "application/xml", fileName);
            }
            catch (Exception ex)
            {
                return Ok();
            }

        }
    }
}
