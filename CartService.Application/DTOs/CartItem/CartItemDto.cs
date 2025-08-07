namespace CartService.Application.DTOs.CartItem
{
    public class CartItemDto
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
