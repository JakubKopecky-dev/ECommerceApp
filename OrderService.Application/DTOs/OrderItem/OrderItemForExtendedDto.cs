namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record OrderItemForExtendedDto
    {
        public Guid Id { get; init; }

        public Guid ProductId { get; init; }

        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

    }
}
