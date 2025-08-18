namespace Shared.Contracts.Events
{
    // unused for now (can be used for orderItems in notification (probably mail))
    public sealed record OrderItemEvent
    {
        public Guid ProductId { get; init; }

        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

    }
}
