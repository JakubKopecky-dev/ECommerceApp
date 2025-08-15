namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record CreateOrderItemDto
    {
        public Guid ProductId { get; init; }

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

        public Guid OrderId { get; init; }

        public string ProductName { get; init; } = "";


    }
}
