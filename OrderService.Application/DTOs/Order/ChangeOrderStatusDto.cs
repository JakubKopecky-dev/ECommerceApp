using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public sealed record ChangeOrderStatusDto
    {
        public OrderStatus Status { get; init; }
    }
}
