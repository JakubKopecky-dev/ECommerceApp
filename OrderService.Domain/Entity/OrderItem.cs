using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Common;

namespace OrderService.Domain.Entity
{
    public class OrderItem : BaseEntity
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal UnitPrice { get; set; }

        public uint Quantity { get; set; }

        public Guid OrderId { get; set; }
        public required Order Order { get; set; }

    }
}
