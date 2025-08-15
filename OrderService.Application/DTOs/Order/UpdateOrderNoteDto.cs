using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.Order
{
    public sealed record UpdateOrderNoteDto
    {
        [MaxLength(1000)]
        public string? Note { get; init; }

    }
}
