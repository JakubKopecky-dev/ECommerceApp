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
    public class OrderRepository(OrderDbContext dbContext) : BaseRepository<Order>(dbContext), IOrderRepository
    {
        public async Task<Order?> FindOrderByIdIncludeOrderItemAsync(Guid orderId) => await _dbSet
                                                                                            .Where(o => o.Id == orderId)
                                                                                            .Include(o => o.Items)
                                                                                            .FirstOrDefaultAsync();

        

    }
}
