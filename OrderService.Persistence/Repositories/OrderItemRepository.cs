using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entity;

namespace OrderService.Persistence.Repositories
{
    public class OrderItemRepository(OrderDbContext dbContext) : BaseRepository<OrderItem>(dbContext),IOrderItemRepository
    {
        public async Task<IReadOnlyList<OrderItem>> GetAllOrderItemsByOrderId(Guid orderId) => await _dbSet
                                                                                                        .Where(oi => oi.OrderId == orderId)
                                                                                                        .ToListAsync();
    }
}
