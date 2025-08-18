using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.DTOs.External;

namespace CartService.Application.Interfaces.External
{
    public interface IOrderReadClient
    {
        Task<Guid?> CreateOrderAndDeliveryAsync(CreateOrderAndDeliveryDto checkOutCartDto, CancellationToken ct = default);
    }
}
