using VideoStreamer.Bootstrapper;
using System.ComponentModel.Composition;
using VideoStreamer.Domain.Services;
using VideoStreamer.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace VideoStreamer.Application
{
    [Export(typeof(IComponent))]
    public class ApplicationComposer : IComponent
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
        }

        public void Register(IComposer composer)
        {
            composer.RegisterType<IUserService, UserService>()
                .RegisterType<IRoleService, RoleService>();
        }
    }
}
