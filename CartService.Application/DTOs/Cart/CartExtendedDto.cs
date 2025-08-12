using CartService.Application.DTOs.CartItem;

namespace CartService.Application.DTOs.Cart
{
    public class CartExtendedDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public List<CartItemDto> Items { get; set; } = [];
    }
}
