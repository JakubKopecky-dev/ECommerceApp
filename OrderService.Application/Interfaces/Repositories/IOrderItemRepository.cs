using OrderService.Domain.Entity;

namespace OrderService.Application.Interfaces.Repositories
{
    public interface IOrderItemRepository : IBaseRepository<OrderItem>
    {
        Task<IReadOnlyList<OrderItem>> GetAllOrderItemsByOrderId(Guid orderId);
    }
}
