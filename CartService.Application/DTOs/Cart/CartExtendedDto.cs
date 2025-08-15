using CartService.Application.DTOs.CartItem;

namespace CartService.Application.DTOs.Cart
{
    public sealed record CartExtendedDto
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public decimal TotalPrice { get; init; }

        public List<CartItemDto> Items { get; init; } = [];
    }
}
