using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        Task<IReadOnlyList<OrderItem>> GetAllOrderItemsByOrderId(Guid orderId, CancellationToken ct = default);
    }
}
