using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Enum;

namespace OrderService.Application.DTOs.Order
{
    public sealed record ChangeInternalOrderStatusDto
    {
        public InternalOrderStatus InternalStatus { get; init; }
    }
}
