using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Contracts.Enums;

namespace OrderService.Application.Interfaces.External
{
    public interface IDeliveryReadClient
    {
        Task<Guid> CreateDeliveryAsync(DTOs.External.CreateDeliveryDto dto, CancellationToken ct = default);
        Task<DeliveryStatus?> GetDeliveryStatusByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    }
}
