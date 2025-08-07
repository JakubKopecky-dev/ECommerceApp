using DeliveryService.Application.Interfaces.Repositories;
using DeliveryService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Persistence.Repository
{
    public class DeliveryRepository(DeliveryDbContext dbContext) : BaseRepository<Delivery>(dbContext), IDeliveryRepository
    {
        public async Task<Delivery?> FindDeliveryByOrderIdIncludeCourierAsync(Guid orderId) => await _dbSet
                                                                                                        .Include(d => d.Courier)
                                                                                                        .FirstOrDefaultAsync(d => d.OrderId == orderId);
    }
}
