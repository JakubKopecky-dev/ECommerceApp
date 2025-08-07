using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.External
{
    public class DeliveryExternalDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public DeliveryStatus Status { get; set; }
    }
}
