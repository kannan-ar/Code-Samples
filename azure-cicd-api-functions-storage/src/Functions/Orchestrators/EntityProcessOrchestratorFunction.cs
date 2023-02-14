using Functions.Orchestrators.Activities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace Functions.Orchestrators
{
    public class EntityProcessOrchestratorFunction
    {
        [FunctionName(nameof(EntityProcesOrchestrator))]
        public async Task EntityProcesOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext)
        {
            var entity = orchestrationContext.GetInput<Modals.Entity>();

            await orchestrationContext.CallActivityAsync(
                nameof(EntityActivityFunctions.SaveEntity), entity);
        }
    }
}
