using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entity;

namespace OrderService.Persistence.Repositories
{
    public class OrderRepository(OrderDbContext dbContext) : BaseRepository<Order>(dbContext), IOrderRepository
    {
        public async Task<Order?> FindOrderByIdIncludeOrderItemAsync(Guid orderId, CancellationToken ct = default) => await _dbSet
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == orderId,ct);




        public async Task<IReadOnlyList<Order>> GetAllOrderByUserIdAsync(Guid userId,CancellationToken ct = default) => await _dbSet
            .Where(o => o.UserId == userId)
            .ToListAsync(ct);




    }
}
