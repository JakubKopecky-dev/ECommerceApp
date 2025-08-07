using CartService.Application.DTOs.CartItem;

namespace CartService.Application.DTOs.Cart
{
    public class CheckoutCartDto
    {
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public List<CartItemForCheckoutDto> Items { get; set; } = [];
    }
}
