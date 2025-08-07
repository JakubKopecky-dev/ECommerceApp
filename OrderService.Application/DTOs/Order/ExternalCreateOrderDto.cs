using OrderService.Application.DTOs.OrderItem;

namespace OrderService.Application.DTOs.Order
{
    public class ExternalCreateOrderDto
    {
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public List<ExternalCreateOrderItemDto> Items { get; set; } = [];
    }
}
