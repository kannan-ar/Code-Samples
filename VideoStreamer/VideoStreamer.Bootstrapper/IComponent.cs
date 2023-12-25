using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VideoStreamer.Bootstrapper
{
    public interface IComponent
    {
        void Register(IComposer composer);
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
