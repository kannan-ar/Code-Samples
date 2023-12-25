using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using Unity;

namespace VideoStreamer.Bootstrapper
{
    public class BootstrapLoader
    {
        private readonly string path;
        private readonly string pattern;

        private IEnumerable<Export> exports;

        public BootstrapLoader(string path, string pattern)
        {
            this.path = path;
            this.pattern = pattern;
        }

        public void Export(Type type)
        {
            var compositionContainer = new CompositionContainer(new AggregateCatalog(new DirectoryCatalog(path, pattern)));

            exports = compositionContainer.GetExports(new ImportDefinition(def => true, type.FullName, ImportCardinality.ZeroOrOne, false, false));
        }

        public void Resolve(IUnityContainer container)
        {
            IEnumerable<IComponent> modules = exports.Select(export => export.Value as IComponent).Where(x => x != null);

            var composer = new UnityComposer(container);

            foreach (var module in modules)
            {
                module.Register(composer);
            }
        }

        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            IEnumerable<IComponent> modules = exports.Select(export => export.Value as IComponent).Where(x => x != null);

            foreach (var module in modules)
            {
                module.Configure(services, configuration);
            }
        }
    }
}
