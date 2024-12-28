using Sync.Engine;
using Sync.Engine.Schema;

namespace Sync.Client;

public class SchemaImport(
    IEnumerable<SchemaImporter<ISchemaImportOptions>> schemas)
{
    public void Import(IDictionary<string, object> options)
    {
        var dictionary = new Dictionary<string, object>(options, StringComparer.OrdinalIgnoreCase);
        var provider = dictionary[nameof(IProvider.Provider)] ?? throw new InvalidOperationException("Provider is not specified");
        var schema = schemas.GetProvider<SchemaImporter<ISchemaImportOptions>>(Convert.ToString(provider) ?? string.Empty);

        schema.LoadOptions(dictionary);
        schema.Import();
    }
}