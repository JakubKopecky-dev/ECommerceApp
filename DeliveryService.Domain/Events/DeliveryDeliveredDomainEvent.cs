using DeliveryService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Domain.Events
{
    public class DeliveryDeliveredDomainEvent : IDomainEvent
    {
        public Guid OrderId { get; init; }

    }
}
