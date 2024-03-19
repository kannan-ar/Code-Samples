using MediaServices.MediaManager.Models;
using System.Diagnostics;
using System.Text;

namespace MediaServices.MediaManager.Services
{
    public class Converter : IConverter
    {
        private readonly IBlobManager _blobManager;
        private string _containerName;

        public Converter(IBlobManager blobManager)
        {
            _blobManager = blobManager;
        }

        public void Convert(string resolutionList, MediaType mediaType, EncoderList encoder, string containerName, string fileName)
        {
            _containerName = containerName;

            var dir = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Video"));

            foreach(var fileInfo in dir.GetFiles())
            {
                fileInfo.Delete();
            }

            var file = Path.Combine(AppContext.BaseDirectory, fileName);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    WorkingDirectory = Path.Combine(AppContext.BaseDirectory, "Video"),
                    Arguments = GetFFMpegParameters(resolutionList, mediaType, encoder, containerName, file),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };

            process.Exited += Process_Exited;
            process.Start();
            process.WaitForExit();

            var error = process.StandardError.ReadToEnd();
            var message = process.StandardOutput.ReadToEnd();
        }

        private string GetFFMpegParameters(string resolutionList, MediaType mediaType, EncoderList encoder, string containerName, string file)
        {
            var parameter = string.Empty;

            switch (mediaType)
            {
                case MediaType.MpegDash:
                    parameter = $"-re -i {file} -map 0 -map 0 {GetFFMpegEncoder(encoder)} {GetResolutionList(resolutionList, mediaType)} -adaptation_sets \"id=0,streams=v id=1,streams=a\" -f dash {containerName}.mpd";
                    break;
                case MediaType.HLS:
                    parameter =  $"-i {file} -profile:v baseline -level 3.0 {GetResolutionList(resolutionList, mediaType)} -start_number 0 -hls_time 10 -hls_list_size 0 -f hls {containerName}.m3u8";
                    break;
                default:
                    parameter = string.Empty;
                    break;
            }

            return parameter;
        }

        private string GetResolutionList(string resolutionList, MediaType mediaType)
        {
            var resolutions = resolutionList.Split(',');
            var sb = new StringBuilder();

            switch(mediaType)
            {
                case MediaType.MpegDash:
                    for (int i = 0; i < resolutions.Length; i++)
                    {
                        sb.Append($"-s:v:{i} {resolutions[i]} ");
                    }
                    break;

                case MediaType.HLS:
                    foreach (var resolution in resolutions)
                    {
                        sb.Append($"-s {resolution} ");
                    }
                    break;
            }
            

            return sb.ToString();
        }

        private string GetFFMpegEncoder(EncoderList encoder)
        {
            switch(encoder)
            {
                case EncoderList.H264:
                    return "-c:a aac -c:v libx264";
                case EncoderList.AV1:
                    return "-c:a libopus -c:v libsvtav1";
                default:
                    return string.Empty;
            }
        }

        private void Process_Exited(object? sender, EventArgs e)
        {
            foreach(var file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Video")))
            {
                Task.Run(async () =>
                {
                    await _blobManager.UploadBlob(_containerName, Path.GetFileName(file), File.Open(file, FileMode.Open));
                });
            }
        }
    }
}
