using CartService.Application.Interfaces.Repositories;
using CartService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence.Repositories
{
    public class CartItemRepository(CartDbContext dbContext) : BaseRepository<CartItem>(dbContext), ICartItemRepository
    {
        public async Task<CartItem?> FindCartItemByCartIdAndProductIdAsync(Guid cartId, Guid productId, CancellationToken ct = default) => await _dbSet
            .Where(c => c.ProductId == productId && c.CartId == cartId)
            .FirstOrDefaultAsync(ct);

    }
}
