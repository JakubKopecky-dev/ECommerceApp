using Shared.Contracts.Enums;


namespace Shared.Contracts.Events
{
    public class OrderStatusChangedEvent
    {
        public Guid OrderId { get; set; }

        public OrderStatus NewStatus { get; set; }

        public Guid UserId { get; set; }

        public DateTime UpdatedAt { get; set; }

    }
}
