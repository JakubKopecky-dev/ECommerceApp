namespace CartService.Application.DTOs.CartItem
{
    public sealed record ChangeQuantityCartItemDto
    {
        public uint Quantity { get; init; }

    }
}
