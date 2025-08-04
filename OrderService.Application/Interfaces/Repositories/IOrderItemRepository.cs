using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Domain.Entity;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        Task<IReadOnlyList<OrderItem>> GetAllOrderItemsByOrderId(Guid orderId);
    }
}
