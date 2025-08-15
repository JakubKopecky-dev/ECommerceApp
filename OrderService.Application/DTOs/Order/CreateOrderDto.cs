using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.Order
{
    public sealed record CreateOrderDto
    {
        public Guid UserId { get; init; }

        [MaxLength(1000)]
        public string? Note { get; init; }
    }
}
