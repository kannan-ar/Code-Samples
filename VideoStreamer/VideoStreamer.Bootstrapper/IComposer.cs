using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VideoStreamer.Bootstrapper
{
    public interface IComposer
    {
        IComposer RegisterType<T, K>() where K : T;
    }
}
