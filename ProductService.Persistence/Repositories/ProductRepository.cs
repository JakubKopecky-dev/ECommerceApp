using Microsoft.EntityFrameworkCore;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Domain.Entities;

namespace ProductService.Persistence.Repositories
{
    public class ProductRepository(ProductDbContext dbContext) : BaseRepository<Product>(dbContext), IProductRepository
    {
        public async Task<Product?> FindProductByIdIncludeCategoriesAsync(Guid productId, CancellationToken ct = default) => await _dbSet
            .Include(c => c.Categories)
            .SingleOrDefaultAsync(c => c.Id == productId, ct);



        public async Task<Product?> FindProductByIdIncludeCategoriesAndReviewsAsync(Guid productId, CancellationToken ct = default) => await _dbSet
            .Include(c => c.Categories)
            .Include(c => c.Reviews)
            .SingleOrDefaultAsync(c => c.Id == productId, ct);



        public async Task<IReadOnlyList<Product>> GetAllProductsByBrandIdAsync(Guid brandId, CancellationToken ct = default) => await _dbSet
            .Where(p => p.BrandId == brandId)
            .ToListAsync(ct);



        public async Task<IReadOnlyList<Product>> GetAllProductsByCategoriesAsync(List<string> categories, CancellationToken ct = default) => await _dbSet
            .Where(p => p.Categories.Any(c => categories.Contains(c.Title)))
            .ToListAsync(ct);



        public async Task<IReadOnlyList<Product>> GetAllActiveProductsAsync(CancellationToken ct = default) => await _dbSet
            .Where(p => p.IsActive).ToListAsync(ct);



        public async Task<IReadOnlyList<Product>> GetAllInactiveProductsAsync(CancellationToken ct = default) => await _dbSet
            .Where(p => !p.IsActive).ToListAsync(ct);



        public async Task<IReadOnlyList<ProductQuantityCheckResponseDto>> GetProductsAsQuantityCheckDtoAsync(List<Guid> productIds, CancellationToken ct = default) => await _dbSet
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new ProductQuantityCheckResponseDto {Id = p.Id,Title = p.Title, QuantityInStock = p.QuantityInStock })
            .ToListAsync(ct);



        public async Task<IReadOnlyList<Product>> GetAllProductsByIdsAsync(List<Guid> productIds, CancellationToken ct = default) => await _dbSet
            .Where(p => productIds.Contains(p.Id))
            .ToListAsync(ct);


    }
}