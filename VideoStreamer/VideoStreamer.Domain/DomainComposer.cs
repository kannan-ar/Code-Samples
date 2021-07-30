using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using VideoStreamer.Bootstrapper;
using VideoStreamer.Domain.Services;

namespace VideoStreamer.Domain
{
    [Export(typeof(IComponent))]
    public class DomainComposer : IComponent
    {
        public void Register(IComposer composer)
        {
            composer.RegisterType<IUserService, UserService>()
                .RegisterType<IRoleService, RoleService>();
        }
    }
}
