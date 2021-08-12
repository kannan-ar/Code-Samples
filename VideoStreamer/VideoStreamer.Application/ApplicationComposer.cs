using VideoStreamer.Bootstrapper;
using System.ComponentModel.Composition;
using VideoStreamer.Domain.Services;
using VideoStreamer.Application.Services;

namespace VideoStreamer.Application
{
    [Export(typeof(IComponent))]
    public class ApplicationComposer : IComponent
    {
        public void Register(IComposer composer)
        {
            composer.RegisterType<IUserService, UserService>()
                .RegisterType<IRoleService, RoleService>();
        }
    }
}
