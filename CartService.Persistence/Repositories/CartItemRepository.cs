using CartService.Application.Interfaces.Repositories;
using CartService.Domain.Entity;

namespace CartService.Persistence.Repositories
{
    public class CartItemRepository(CartDbContext dbContext) : BaseRepository<CartItem>(dbContext), ICartItemRepository
    {
    }
}
