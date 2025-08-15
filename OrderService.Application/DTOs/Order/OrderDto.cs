using System.ComponentModel.DataAnnotations;
using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public sealed record OrderDto
    {
        public Guid Id { get; init; }

        public Guid UserId { get; init; }

        public decimal TotalPrice { get; init; }

        public OrderStatus Status { get; init; }

        [MaxLength(1000)]
        public string? Note { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
