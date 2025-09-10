namespace PaymentService.Api.DTOs
{
    public sealed record OrderItemDto
    {
        public string ProductName { get; init; } = "";

        public uint Quantity { get; init; }

        public decimal UnitPrice { get; init; }
    }
}
