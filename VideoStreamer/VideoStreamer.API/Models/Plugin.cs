using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoStreamer.API.Models
{
    public class Plugin
    {
        public string Path { get; set; }
        public IEnumerable<string> Patterns { get; set; }
    }
}
