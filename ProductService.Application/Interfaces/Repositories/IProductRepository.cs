using ProductService.Application.DTOs.Product;
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
        Task<IReadOnlyList<ProductQuantityCheckResponseDto>> GetProductsAsQuantityCheckDtoAsync(List<Guid> productIds, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetAllProductsByIdsAsync(List<Guid> productIds, CancellationToken ct = default);
    }
}
