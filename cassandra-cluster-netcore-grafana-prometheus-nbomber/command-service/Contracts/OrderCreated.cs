
using ProtoBuf;

namespace CommandServiceApi.Contracts
{
    [ProtoContract]
    public record OrderCreated : IOrderEvent
    {
        [ProtoMember(1)]
        public string EventType => nameof(OrderCreated);

        [ProtoMember(2)]
        public Guid OrderId { get; set; }

        [ProtoMember(3)]
        public decimal Amount { get; set; }

    }
}
