using MassTransit;

namespace Messaging.Lib
{
    public class OutboxState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
    }
}
