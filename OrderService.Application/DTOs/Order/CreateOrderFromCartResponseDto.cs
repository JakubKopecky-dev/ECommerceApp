using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.Order
{
    public sealed record CreateOrderFromCartResponseDto
    {
        public Guid OrderId { get; set; }

        public Guid? DeliveryId { get; set; }

        public string? CheckoutUrl { get; set; } = "";
    }
}
