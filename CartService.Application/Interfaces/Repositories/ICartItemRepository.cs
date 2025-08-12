using CartService.Domain.Entity;

namespace CartService.Application.Interfaces.Repositories
{
    public interface ICartItemRepository : IBaseRepository<CartItem>
    {
        Task<CartItem?> FindCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId,CancellationToken ct = default);
    }
}
