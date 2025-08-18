using Shared.Contracts.Enums;


namespace Shared.Contracts.Events
{
    public sealed record OrderStatusChangedEvent
    {
        public Guid OrderId { get; init; }

        public OrderStatus NewStatus { get; init; }

        public Guid UserId { get; init; }

        public DateTime UpdatedAt { get; init; }

    }
}
