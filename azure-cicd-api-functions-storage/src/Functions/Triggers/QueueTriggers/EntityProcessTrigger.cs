using Azure.Storage.Queues.Models;
using Functions.Orchestrators;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Text.Json;
using System.Threading.Tasks;

namespace Functions.Triggers.QueueTriggers
{
    public class EntityProcessTrigger
    {
        [FunctionName(nameof(EntityProcess))]
        public async Task EntityProcess(
            [QueueTrigger("%QueueNameSettings:ProcessEntityQueueName%", Connection = "ConnectionStrings:QueueStorage")] QueueMessage queueMessage,
            [DurableClient] IDurableOrchestrationClient durableOrchestrationClient)
        {
            var entity = JsonSerializer.Deserialize<Modals.Entity>(queueMessage.MessageText);
            await durableOrchestrationClient.StartNewAsync(
                nameof(EntityProcessOrchestratorFunction.EntityProcesOrchestrator), entity);
        }
    }
}
