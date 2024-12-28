using Sync.Engine;

namespace Sync.Client;

public static class ClientExtensions
{
    public static T GetProvider<T>(this IEnumerable<IProvider> providers, string providerName)
        where T : class
    {
        return providers.FirstOrDefault(x => string.Equals(providerName, x.Provider, StringComparison.OrdinalIgnoreCase)) as T
            ?? throw new InvalidOperationException($"Provider {typeof(T)} not registered");
    }
}
