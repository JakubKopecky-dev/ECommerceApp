using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entity;

namespace OrderService.Persistence.Repositories
{
    public class OrderItemRepository(OrderDbContext dbContext) : BaseRepository<OrderItem>(dbContext),IOrderItemRepository
    {    
    }
}
