using Functions.Modals;
using Functions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Functions.Orchestrators.Activities
{
    public class EntityActivityFunctions
    {
        private readonly ITableService tableService;
        private readonly TableNameSettings tableNameSettings;

        public EntityActivityFunctions(
            ITableService tableService,
            IOptions<TableNameSettings> tableNameSettings)
        {
            this.tableService = tableService;
            this.tableNameSettings = tableNameSettings.Value;
        }

        [FunctionName(nameof(SaveEntity))]
        public async Task SaveEntity([ActivityTrigger] Modals.Entity entity, ExecutionContext executionContext)
        {
            await tableService.Insert(tableNameSettings.EntityTableName, entity);
        }
    }
}
