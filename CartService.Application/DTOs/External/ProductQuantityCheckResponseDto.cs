using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs.External
{
    public sealed record ProductQuantityCheckResponseDto
    {
        public Guid Id { get; init; }

        public string Title { get; init; } = "";

        public uint QuantityInStock { get; init; }
    }
}
