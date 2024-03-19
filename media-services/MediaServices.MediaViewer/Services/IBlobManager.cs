namespace MediaServices.MediaViewer.Services
{
    public interface IBlobManager
    {
        Task<Stream> GetBlob(string containerName, string fileName);
    }
}
