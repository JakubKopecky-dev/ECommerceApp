namespace CartService.Application.DTOs.CartItem
{
    public sealed record CreateCartItemDto
    {
        public Guid CartId { get; init; }

        public Guid ProductId { get; init; }

        public uint Quantity { get; init; }

    }
}
