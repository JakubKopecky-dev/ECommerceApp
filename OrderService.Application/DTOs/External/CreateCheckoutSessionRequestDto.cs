using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.DTOs.External;

namespace OrderService.Application.DTOs.External
{
    public sealed record CreateCheckoutSessionRequestDto
    {
        /// <summary>
        /// OrderId
        /// </summary>
        public Guid Id {  get; init; }

        /// <summary>
        /// orderItems
        /// </summary>
        public List<OrderItemForCreateCheckoutSessionRequestDto> Items { get; init; } = [];
    }
}
