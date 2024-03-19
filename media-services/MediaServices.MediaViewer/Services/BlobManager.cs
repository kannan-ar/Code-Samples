
using Azure.Storage.Blobs;

namespace MediaServices.MediaViewer.Services
{
    public class BlobManager : IBlobManager
    {
        private readonly BlobServiceClient _blobClient;

        public BlobManager(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task<Stream> GetBlob(string containerName, string fileName)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            var cloudBlob = containerClient.GetBlobClient(fileName);

            var stream = new MemoryStream();
            await cloudBlob.DownloadToAsync(stream);
            stream.Position = 0;
    
            return stream;
        }
    }
}
