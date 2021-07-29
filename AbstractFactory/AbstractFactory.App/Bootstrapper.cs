using System;
using System.ComponentModel.Composition.Hosting;
using Unity;

namespace AbstractFactory.App
{
    public interface IComposer
    {
        IComposer Compose(string path, string pattern);
        IComposer RegisterTypeWithEnum<T, K>() where K : Enum;
    }

    public class UnityComposer : IComposer
    {
        private readonly IUnityContainer container;
        private CompositionContainer compositionContainer;

        public UnityComposer(IUnityContainer container)
        {
            this.container = container;
        }

        public IComposer Compose(string path, string pattern)
        {
            compositionContainer = new CompositionContainer(new AggregateCatalog(new DirectoryCatalog(path, pattern)));
            return this;
        }

        public IComposer RegisterTypeWithEnum<T, K>() where K : Enum
        {
            foreach(var name in Enum.GetNames(typeof(K)))
            {
                var export = compositionContainer.GetExport<T>(name);

                container.RegisterType(typeof(T), export.Value.GetType(), name);
            }

            return this;
        }
    }

    public static class Bootstrapper
    {
       public static IComposer UseUnity(this IUnityContainer container, string path, string pattern)
        {
            return new UnityComposer(container).Compose(path, pattern);
        }
    }
}
