using ProductService.Domain.Entity;

namespace ProductService.Application.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Product?> FindProductByIdIncludeCategoriesAsync(Guid productId, CancellationToken ct = default);
        Task<Product?> FindProductByIdIncludeCategoriesAndReviewsAsync(Guid productId, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllProductsByBrandIdAsync(Guid brandId, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllProductsByCategoriesAsync(List<string> categories, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllActiveProductsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllInactiveProductsAsync(CancellationToken ct = default);
    }
}
