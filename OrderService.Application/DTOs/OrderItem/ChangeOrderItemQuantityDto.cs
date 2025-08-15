namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record ChangeOrderItemQuantityDto
    {
        public uint Quantity { get; init; }
    }
}
