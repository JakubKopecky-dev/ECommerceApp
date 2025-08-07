using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entity;

namespace ProductService.Persistence.Repositories
{
    public class ProductRepository(ProductDbContext dbContext) : BaseRepository<Product>(dbContext), IProductRepository
    {
        public async Task<Product?> FindProductByIdIncludeCategoriesAsync(Guid productId) => await _dbSet
                                                                                                .Include(c => c.Categories)
                                                                                                .FirstOrDefaultAsync(c => c.Id == productId);


        public async Task<Product?> FindProductByIdIncludeCategoriesAndReviewsAsync(Guid productId) => await _dbSet
                                                                                                .Include(c => c.Categories)
                                                                                                .Include (c => c.Reviews)
                                                                                                .FirstOrDefaultAsync(c => c.Id == productId);

    }
}
