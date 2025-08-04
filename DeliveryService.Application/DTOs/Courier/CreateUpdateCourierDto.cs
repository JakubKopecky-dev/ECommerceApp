using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application.DTOs.Courier
{
    public class CreateUpdateCourierDto
    {
        public string Name { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }
    }
}
