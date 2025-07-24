using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class CreateOrderDto
    {
        public Guid UserId { get; set; }

        [MaxLength(1000)]
        public string? Note { get; set; }
    }
}
