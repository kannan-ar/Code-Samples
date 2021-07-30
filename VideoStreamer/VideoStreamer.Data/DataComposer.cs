using VideoStreamer.Data.Repositories;
using VideoStreamer.Bootstrapper;
using System.ComponentModel.Composition;

namespace VideoStreamer.Data
{
    [Export(typeof(IComponent))]
    public class DataComposer : IComponent
    {
        public void Register(IComposer composer)
        {
            composer.RegisterType<IUserRepository, UserRepository>()
                .RegisterType<IRoleRepository, RoleRepository>();
        }
    }
}
