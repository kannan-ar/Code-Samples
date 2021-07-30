using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamer.Bootstrapper
{
    public interface IComposer
    {
        IComposer RegisterType<T, K>() where K : T;
    }
}
