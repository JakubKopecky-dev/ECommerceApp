using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record CreateOrderItemDto
    {
        public Guid ProductId { get; init; }

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }

        public Guid OrderId { get; init; }

        [MaxLength(500)]
        public string ProductName { get; init; } = "";


    }
}
