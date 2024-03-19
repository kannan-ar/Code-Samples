using MediaServices.MediaManager.Models;

namespace MediaServices.MediaManager.Services
{
    public interface IConverter
    {
        void Convert(string resolutionList, MediaType mediaType, EncoderList encoder, string containerName, string fileName);
    }
}
