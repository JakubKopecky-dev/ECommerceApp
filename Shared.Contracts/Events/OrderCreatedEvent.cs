using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }

    }
}
