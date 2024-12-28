using System.ComponentModel.DataAnnotations;

namespace Sync.Engine.Schema;

public interface ISchemaImportOptions
{
    string Provider { get; set; }
   
}
