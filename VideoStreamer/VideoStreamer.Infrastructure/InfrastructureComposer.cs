using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Composition;
using VideoStreamer.Bootstrapper;
using VideoStreamer.Infrastructure.Repositories;
using VideoStreamer.Infrastructure.Settings;

namespace VideoStreamer.Infrastructure
{
    [Export(typeof(IComponent))]
    internal class InfrastructureComposer : IComponent
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DbSettings>(configuration.GetSection("DbSettings"));
        }

        public void Register(IComposer composer)
        {
            composer.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}
