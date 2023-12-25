using AutoMapper.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Unity;

namespace VideoStreamer.Bootstrapper
{
    public class UnityComposer : IComposer
    {
        private readonly IUnityContainer container;

        public UnityComposer(IUnityContainer container)
        {
            this.container = container;
        }

        public IComposer RegisterType<T, K>() where K : T
        {
            container.RegisterType<T, K>();
            return this;
        }
    }
}
