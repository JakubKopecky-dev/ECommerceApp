using Shared.Contracts.DTOs;

namespace Shared.Contracts.Events
{
    public sealed record OrderCreatedEvent
    {
        public Guid OrderId { get; init; }

        public Guid UserId { get; init; }

        public decimal TotalPrice { get; init; }

        public string? Note { get; init; }

        public DateTime CreatedAt { get; init; }

    }
}
