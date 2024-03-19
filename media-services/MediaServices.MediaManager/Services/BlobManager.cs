using Azure.Storage.Blobs;

namespace MediaServices.MediaManager.Services
{
    public class BlobManager : IBlobManager
    {
        private readonly BlobServiceClient _blobClient;

        public BlobManager(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }

        public async Task UploadBlob(string containerName, string fileName, Stream stream)
        {
            var containerClient = _blobClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream);
        }

        //public void UploadBlob(string containerName, string fileName, Stream stream)
        //{
        //    var containerClient = _blobClient.GetBlobContainerClient(containerName);
        //    containerClient.CreateIfNotExists();

        //    var blobClient = containerClient.GetBlobClient(fileName);
        //    var response = blobClient.Upload(stream);
        //}
    }
}
