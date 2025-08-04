using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Domain.Enum;

namespace DeliveryService.Application.DTOs.Delivery
{
    public class ChangeDeliveryStatusDto
    {
        public DeliveryStatus Status { get; set; }

    }
}
