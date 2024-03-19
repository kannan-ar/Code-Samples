using MediaServices.MediaManager.Models;

namespace MediaServices.MediaManager.Services
{
    public interface ITranscoder
    {
        Task Transcode(string resolutionList, MediaType mediaType, EncoderList encoder, string containerName, string fileName, Stream stream);
    }
}
