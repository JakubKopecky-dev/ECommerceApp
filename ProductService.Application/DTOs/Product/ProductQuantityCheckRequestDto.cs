using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs.Product
{
    public sealed record ProductQuantityCheckRequestDto
    {
        public Guid Id { get; init; }

        public uint Quantity { get; init; }
    }
}
