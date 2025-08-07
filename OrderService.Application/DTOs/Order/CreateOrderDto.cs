using System.ComponentModel.DataAnnotations;

namespace OrderService.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }
    }
}
