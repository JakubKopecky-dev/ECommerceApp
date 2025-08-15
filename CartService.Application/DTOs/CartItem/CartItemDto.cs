namespace CartService.Application.DTOs.CartItem
{
    public sealed record CartItemDto
    {
        public Guid Id { get; init; }

        public Guid CartId { get; init; }

        public Guid ProductId { get; init ; }

        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
