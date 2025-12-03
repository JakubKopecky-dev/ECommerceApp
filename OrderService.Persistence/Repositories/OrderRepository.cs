using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Persistence.Repositories
{
    public class OrderRepository(OrderDbContext dbContext) : BaseRepository<Order>(dbContext), IOrderRepository
    {
        public async Task<Order?> FindOrderByIdIncludeOrderItemAsync(Guid orderId, CancellationToken ct = default) => await _dbSet
            .Include(o => o.Items)
            .SingleOrDefaultAsync(o => o.Id == orderId,ct);




        public async Task<IReadOnlyList<Order>> GetAllOrderByUserIdAsync(Guid userId,CancellationToken ct = default) => await _dbSet
            .Where(o => o.UserId == userId)
            .ToListAsync(ct);



        public async Task<IReadOnlyList<Order>> GetAllOrderStatusWithDeliveryFaildInternalStatus(CancellationToken ct = default) => await _dbSet
            .Where(o => o.InternalStatus == InternalOrderStatus.DeliveryFaild)
            .ToListAsync(ct);



    }
}
