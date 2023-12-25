using System.Collections.Generic;

namespace VideoStreamer.Bootstrapper
{
    public class Plugin
    {
        public string Path { get; set; }
        public IEnumerable<string> ComposerPatterns { get; set; }
        public IEnumerable<string> MapperPatterns { get; set; }
    }
}
