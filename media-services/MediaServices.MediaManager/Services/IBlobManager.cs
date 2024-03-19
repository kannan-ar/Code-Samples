namespace MediaServices.MediaManager.Services
{
    public interface IBlobManager
    {
        Task UploadBlob(string containerName, string fileName, Stream stream);
        //void UploadBlob(string containerName, string fileName, Stream stream);
    }
}
