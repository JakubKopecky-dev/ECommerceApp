using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;

namespace OrderService.Persistence.Repositories
{
    public class OrderItemRepository(OrderDbContext dbContext) : BaseRepository<OrderItem>(dbContext),IOrderItemRepository
    {
        public async Task<IReadOnlyList<OrderItem>> GetAllOrderItemsByOrderId(Guid orderId, CancellationToken ct = default) => await _dbSet
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync(ct);
    }
}
