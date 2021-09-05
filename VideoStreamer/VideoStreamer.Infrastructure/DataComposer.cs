using VideoStreamer.Infrastructure.Repositories;
using VideoStreamer.Bootstrapper;
using System.ComponentModel.Composition;


namespace VideoStreamer.Infrastructure
{
    [Export(typeof(IComponent))]
    public class DataComposer : IComponent
    {
        public void Register(IComposer composer)
        {
            composer.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}
