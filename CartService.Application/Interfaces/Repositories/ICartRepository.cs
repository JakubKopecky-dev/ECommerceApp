using CartService.Domain.Entity;

namespace CartService.Application.Interfaces.Repositories
{
    public interface ICartRepository : IBaseRepository<Cart>
    {
        Task<Cart?> FindCartByUserIdIncludeItemsAsync(Guid userId, CancellationToken ct = default);
    }
}
