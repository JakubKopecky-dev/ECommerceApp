using OrderService.Domain.Common;
using OrderService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Events
{
    public class OrderStatusChangedDomainEvent : IDomainEvent
    {
        public Guid OrderId { get; init; }
        public Guid UserId { get; init; }
        public OrderStatus NewStatus { get; init; }
    }
}
