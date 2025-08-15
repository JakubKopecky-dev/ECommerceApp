using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Contracts.DTOs
{
    public sealed record OrderItemCreatedDto
    {
        public Guid ProductId { get; set; }

        public uint Quantity { get; set; }
    }
}
