using CartService.Application.Interfaces.Repositories;
using CartService.Domain.Entity;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence.Repositories
{
    public class CartRepository(CartDbContext dbContext) : BaseRepository<Cart>(dbContext), ICartRepository
    {
        public async Task<Cart?> FindCartByUserIdIncludeItemsAsync(Guid userId, CancellationToken ct = default) => await _dbSet
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId,ct);
    }
}
