using Shared.Contracts.DTOs;

namespace Shared.Contracts.Events
{
    public sealed record OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
