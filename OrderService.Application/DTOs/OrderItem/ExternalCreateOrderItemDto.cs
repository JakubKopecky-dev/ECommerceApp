using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.OrderItem
{
    public sealed record ExternalCreateOrderItemDto
    {
        public Guid ProductId { get; init; }

        [MaxLength(500)]
        public string ProductName { get; init; } = "";

        public decimal UnitPrice { get; init; }

        public uint Quantity { get; init; }
    }
}
