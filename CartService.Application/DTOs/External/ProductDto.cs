using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs.External
{
    public sealed record ProductDto
    {
        public string Title { get; init; } = "";

        public uint QuantityInStock { get; init; }

        public decimal Price { get; init; }

    }
}
