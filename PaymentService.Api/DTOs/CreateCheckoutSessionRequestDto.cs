namespace PaymentService.Api.DTOs
{
    public sealed record CreateCheckoutSessionRequestDto
    {
        public Guid OrderId { get; init; }

        public List<OrderItemDto> Items { get; init; } = [];

    }
}
