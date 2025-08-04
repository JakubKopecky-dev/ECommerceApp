using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.OrderItem
{
    public class CreateOrderItemDto
    {
        public Guid ProductId { get; set; }

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

        public Guid OrderId { get; set; }

        public string ProductName { get; set; } = "";


    }
}
