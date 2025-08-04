using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Common;
using OrderService.Domain.Enum;

namespace OrderService.Domain.Entity
{
    public class Order : BaseEntity
    {
        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }

        public Guid? DeliveryId { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];
    }
}
