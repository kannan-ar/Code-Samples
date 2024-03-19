using MediaServices.MediaManager.Models;

namespace MediaServices.MediaManager.Services
{
    public class Transcoder : ITranscoder
    {
        private readonly IConverter _converter;

        public Transcoder(IConverter converter)
        {
            _converter = converter;
        }

        public async Task Transcode(string resolutionList, MediaType mediaType, EncoderList encoder, string containerName, string fileName, Stream stream)
        {
            using (var writeStream = new FileStream(Path.Combine(AppContext.BaseDirectory, fileName), FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(writeStream);
            }

            _converter.Convert(resolutionList, mediaType, encoder, containerName, fileName);
        }
    }
}
