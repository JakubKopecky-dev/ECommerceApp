namespace CartService.Application.DTOs.CartItem
{
    public class CreateCartItemDto
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        
        public uint Quantity { get; set; }

    }
}
