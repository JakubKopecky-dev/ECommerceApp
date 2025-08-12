using OrderService.Domain.Entity;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Order?> FindOrderByIdIncludeOrderItemAsync(Guid orderId,CancellationToken ct = default);
        Task<IReadOnlyList<Order>> GetAllOrderByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
