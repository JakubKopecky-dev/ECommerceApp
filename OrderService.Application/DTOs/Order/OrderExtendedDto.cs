using System.ComponentModel.DataAnnotations;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class OrderExtendedDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public List<OrderItemForExtendedDto> Items { get; set; } = [];

    }
}
