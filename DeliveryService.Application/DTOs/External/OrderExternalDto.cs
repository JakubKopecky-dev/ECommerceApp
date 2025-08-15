using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application.DTOs.External
{
    public sealed record OrderExternalDto
    {
        public Guid UserId { get; init; }
    }
}
