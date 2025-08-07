namespace CartService.Application.DTOs.CartItem
{
    public class CartItemForCheckoutDto
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

    }
}
