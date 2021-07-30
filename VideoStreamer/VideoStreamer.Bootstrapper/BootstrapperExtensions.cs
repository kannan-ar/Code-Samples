using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Reflection;
using Unity;
using AutoMapper;

namespace VideoStreamer.Bootstrapper
{
    public static class BootstrapperExtensions
    {
        private static IEnumerable<Export> GetExports(Type type, string path, string pattern)
        {
            var dir = new DirectoryCatalog(path, pattern);

            var compositionContainer = new CompositionContainer(new AggregateCatalog(new DirectoryCatalog(path, pattern)));
            return compositionContainer.GetExports(new ImportDefinition(def => true, type.FullName, ImportCardinality.ZeroOrOne, false, false));
        }

        private static IEnumerable<Type> GetFilteredTypes(Assembly assembly, string nameSpace)
        {
            var types = assembly.GetTypes().Where(t => t.IsClass);

            return string.IsNullOrWhiteSpace(nameSpace) ? types : types.Where(t => t.Namespace == nameSpace);
        }

        public static void Resolve(this IUnityContainer container, string path, string pattern)
        {
            IEnumerable<Export> exports = GetExports(typeof(IComponent), path, pattern);
            IEnumerable<IComponent> modules = exports.Select(export => export.Value as IComponent).Where(x => x != null);

            var composer = new UnityComposer(container);

            foreach (var module in modules)
            {
                module.Register(composer);
            }
        }

        public static void ConfigureMap(this IMapperConfigurationExpression mapper, string path, string pattern)
        {
            IEnumerable<Export> exports = GetExports(typeof(IMapper), path, pattern);
            IEnumerable<IMapper> modules = exports.Select(export => export.Value as IMapper).Where(x => x != null);

            foreach (var module in modules)
            {
                var context = module.CreateMapContext();

                var sourceTypes = GetFilteredTypes(context.Source.Assembly, context.Source.Namespace);
                var targetTypes = GetFilteredTypes(context.Target.Assembly, context.Target.Namespace);

                foreach (var sourceType in sourceTypes)
                {
                    var targetType = targetTypes.Where(x =>
                        string.Concat(context.Target.Prefix, x.Name, context.Target.Postfix) ==
                        string.Concat(context.Source.Prefix, sourceType.Name, context.Source.Postfix)).FirstOrDefault();

                    if (targetType != null)
                    {
                        mapper.CreateMap(sourceType, targetType);
                    }
                }
            }
        }
    }
}
