using OrderService.Domain.Enums;

namespace OrderService.Application.DTOs.Order
{
    public sealed record ChangeOrderStatusDto
    {
        public OrderStatus Status { get; init; }
    }
}
