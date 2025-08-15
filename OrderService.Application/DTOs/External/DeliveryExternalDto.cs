using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.External
{
    public sealed record DeliveryExternalDto
    {
        public Guid Id { get; init; }
        public Guid OrderId { get; init; }
        public DeliveryStatus Status { get; init; }
    }
}
