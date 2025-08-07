using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class ChangeOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
