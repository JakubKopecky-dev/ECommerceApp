using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Application.Interfaces.External
{
    public interface IOrderReadClient
    {
        Task<Guid?> GetUserIdByOrderIdAsync(Guid orderId, CancellationToken ct = default);
    }
}
