using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Domain.Enum;

namespace DeliveryService.Application.DTOs.Delivery
{
    public class DeliveryExtendedDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public CourierDto? Courier { get; set; }

        public DeliveryStatus Status { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public string Street { get; set; } = "";

        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string State { get; set; } = "";

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
