using MediaServices.MediaManager.Models;
using MediaServices.MediaManager.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace MediaServices.MediaManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITranscoder _rePackager;

        public HomeController(ITranscoder rePackager)
        {
            _rePackager = rePackager;
        }


        public IActionResult Index()
        {
            return View(new UploadVideo());
        }

        [HttpPost]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Index(UploadVideo modal)
        {
            var id = Guid.NewGuid().ToString().Replace("-", string.Empty);

            using var stream = modal.Video.OpenReadStream();
            await _rePackager.Transcode(modal.ResolutionList, modal.MediaType, modal.Encoder, id, modal.Video.FileName, stream);

            return View(new UploadVideo
            {
                Name = id
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
