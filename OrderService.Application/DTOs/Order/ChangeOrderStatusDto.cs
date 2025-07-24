using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public class ChangeOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}
