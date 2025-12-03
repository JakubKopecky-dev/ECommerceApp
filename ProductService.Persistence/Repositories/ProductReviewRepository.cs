using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Persistence.Repositories
{
    public class ProductReviewRepository(ProductDbContext dbContext) : BaseRepository<ProductReview>(dbContext), IProductReviewRepository
    {
        public async Task<IReadOnlyList<ProductReview>> GetAllProductReviewsByProductIdAsync(Guid productId, CancellationToken ct = default) => await _dbSet
            .Where(r => r.ProductId == productId)
            .ToListAsync(ct);
                                                       
    }
}
