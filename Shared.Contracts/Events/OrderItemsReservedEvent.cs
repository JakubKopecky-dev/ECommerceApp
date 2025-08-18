using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Contracts.DTOs;

namespace Shared.Contracts.Events
{
    public sealed record OrderItemsReservedEvent
    {
        public Guid OrderId { get; init; }
        public IReadOnlyList<OrderItemCreatedDto> Items { get; init; } = [];
    }
}
