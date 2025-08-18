using ProductService.Application.DTOs.Product;
using Shared.Contracts.DTOs;

namespace ProductService.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> ActivateProductAsync(Guid productId, CancellationToken ct = default);
        Task<ProductExtendedDto> CreateProductAsync(CreateProductDto createDto, CancellationToken ct = default);
        Task<ProductDto?> DeleteProductAsync(Guid productId, CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetAllActiveProductsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetAllInactiveProductsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetAllProductsByBrandIdAsync(Guid brandId, CancellationToken ct = default);
        Task<IReadOnlyList<ProductDto>> GetAllProductsByCategoriesAsync(List<string> categories, CancellationToken ct = default);
        Task<ProductExtendedDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default);
        Task<ProductDto?> InactivateProductAsync(Guid productId, CancellationToken ct = default);
        Task<IReadOnlyList<ProductQuantityCheckResponseDto>> ProductsQuantityCheckFromCartAsync(List<ProductQuantityCheckRequestDto> productsFromCart, CancellationToken ct = default);
        Task ProductQuantityReserved(List<OrderItemCreatedDto> orderItemsDto, CancellationToken ct = default);
        Task<ProductExtendedDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto, CancellationToken ct = default);
    }
}
