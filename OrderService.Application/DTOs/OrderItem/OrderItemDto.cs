namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record OrderItemDto
    {
        public Guid Id { get; init; }

        public Guid ProductId { get; init; }

        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

        public Guid OrderId { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }

    }
}
