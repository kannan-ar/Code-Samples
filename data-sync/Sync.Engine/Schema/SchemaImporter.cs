using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Sync.Engine.Schema;

public abstract class SchemaImporter<Options> : IProvider
    where Options : ISchemaImportOptions
{
    private Options? ImportOptions;
    public abstract string Provider { get; }

    private static IEnumerable<string> GetMissingProperties(IDictionary<string, object> options, IEnumerable<PropertyInfo> properties)
    {
        var requiredType = typeof(RequiredAttribute);

        var requiredProperties = properties.Where(x => x.GetCustomAttributes(requiredType, false).Length > 0);
        var requiredPropertyNames = new HashSet<string>(requiredProperties.Select(x => x.Name));

        return requiredPropertyNames.Where(x => !options.ContainsKey(x));
    }

    public void LoadOptions(IDictionary<string, object> options)
    {
        var properties = typeof(Options).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var missingProperties = GetMissingProperties(options, properties);

        if (missingProperties.Any())
        {
            throw new InvalidOperationException($"{string.Join(", ", missingProperties)} properties are missing");
        }

        foreach (var option in options)
        {
            var property = properties.FirstOrDefault(x => string.Equals(option.Key, x.Name, StringComparison.OrdinalIgnoreCase));

            property?.SetValue(ImportOptions, option.Value);
        }
    }

    public void Import()
    {
        if (ImportOptions == null) throw new InvalidOperationException("Options is empty. Call LoadOptions first");

        Import(ImportOptions);
    }

    protected abstract void Import(Options options);
}
