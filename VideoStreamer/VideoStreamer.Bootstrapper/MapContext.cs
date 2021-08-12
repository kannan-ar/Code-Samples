using System.Reflection;

namespace VideoStreamer.Bootstrapper
{
    public class MapContextInfo
    {
        public Assembly Assembly { get; set; }
        public string Namespace { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }
    }

    public class MapContext
    {
        public MapContextInfo Source { get; set; }
        public MapContextInfo Target { get; set; }
    }
}
