using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application.DTOs.Delivery
{
    public class CreateUpdateDeliveryDto
    {
        public Guid OrderId { get; set; }

        public Guid? CourierId { get; set; }

        public string Street { get; set; } = "";

        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string State { get; set; } = "";
    }
}
