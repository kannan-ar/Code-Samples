namespace MediaServices.MediaManager.Models
{
    public class UploadVideo
    {
        public string ResolutionList {  get; set; }
        public MediaType MediaType { get; set; }
        public EncoderList Encoder { get; set; }
        public string Name { get; set; }
        public IFormFile Video { get; set; }
    }
}
