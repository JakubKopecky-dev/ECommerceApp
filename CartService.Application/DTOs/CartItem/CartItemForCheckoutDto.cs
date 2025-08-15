namespace CartService.Application.DTOs.CartItem
{
    public sealed record CartItemForCheckoutDto
    {
        public Guid ProductId { get; init; }

        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

    }
}
