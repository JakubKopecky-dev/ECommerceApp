using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.External
{
    public sealed record OrderItemForCreateCheckoutSessionRequestDto
    {
        public string ProductName { get; init; } = "";

        public uint Quantity { get; init; }

        public decimal UnitPrice { get; init; }

    }
}
