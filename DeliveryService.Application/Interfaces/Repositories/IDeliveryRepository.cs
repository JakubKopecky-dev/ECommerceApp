using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Domain.Entity;

namespace DeliveryService.Application.Interfaces.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<Delivery?> FindDeliveryByOrderIdIncludeCourierAsync(Guid orderId);
    }
}
