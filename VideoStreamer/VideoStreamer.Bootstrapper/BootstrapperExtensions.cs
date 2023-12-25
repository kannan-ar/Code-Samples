using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Unity;

namespace VideoStreamer.Bootstrapper
{
    public static class BootstrapperExtensions
    {
        public static IUnityContainer Bootstrap(this IServiceCollection services, IConfiguration configuration, Plugin plugin)
        {
            IUnityContainer unityContainer = new UnityContainer();

            foreach (var pattern in plugin.ComposerPatterns)
            {
                var loader = new BootstrapLoader(plugin.Path, pattern);

                loader.Export(typeof(IComponent));

                loader.Resolve(unityContainer);
                loader.Configure(services, configuration);
            }

            services.AddAutoMapper(cfg =>
            {
                foreach (var pattern in plugin.MapperPatterns)
                {
                    ConfigureMapper(cfg, plugin.Path, pattern);
                }
            });

            return unityContainer;
        }

        private static void ConfigureMapper(IMapperConfigurationExpression mapper, string path, string pattern)
        {
            var assembly = Assembly.LoadFrom(Path.Combine(path, pattern));

            if (assembly == null) return;

            mapper.AddMaps(assembly);
        }
    }
}
