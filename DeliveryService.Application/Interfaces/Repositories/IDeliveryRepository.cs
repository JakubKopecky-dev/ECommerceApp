using DeliveryService.Domain.Entities;

namespace DeliveryService.Application.Interfaces.Repositories
{
    public interface IDeliveryRepository : IBaseRepository<Delivery>
    {
        Task<Delivery?> FindDeliveryByOrderIdIncludeCourierAsync(Guid orderId, CancellationToken ct = default);
    }
}
