using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entities;
using Shared.Contracts.DTOs;


namespace ProductService.Application.Services
{
    public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductReviewRepository productReviewRepository, ILogger<ProductService> logger) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly ILogger<ProductService> _logger = logger;



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all products. Count: {Count}.", products.Count);

            return [.. products.Select(x => x.ProductToProductDto())];
        }



        public async Task<ProductExtendedDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving product. ProductId: {ProductId}.", productId);
            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAsync(productId, ct);
            if (product is null)
            {
                _logger.LogInformation("Product not found. ProductId: {ProductId}.", productId);
                return null;
            }

            _logger.LogInformation("Product found. ProductId: {ProductId}.", productId);

            return product.ProductToProductExtendedDto();
        }



        public async Task<ProductExtendedDto> CreateProductAsync(CreateProductDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new product. Title: {Title}.", createDto.Title);

            Product product = Product.Create(createDto.Title,createDto.Description,createDto.Price,createDto.ImageUrl,createDto.BrandId);
            product.AddStock(createDto.QuantityInStock);

            List<Category> categories = await _categoryRepository.GetCategoriesByName(createDto.Categories, ct);

            product.SetCategories(categories);

            await _productRepository.AddAsync(product, ct);
            await _productRepository.SaveChangesAsync(ct);

            _logger.LogInformation("Product created. ProductId: {ProductId}", product.Id);


            return product.ProductToProductExtendedDto();
        }



        public async Task<ProductExtendedDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating product. ProductId: {ProductId}", productId);

            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot update. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            product.Update(updateDto.Title, updateDto.Description, updateDto.Price, updateDto.ImageUrl, updateDto.BrandId);

            IReadOnlyList<Category> categories = await _categoryRepository.GetCategoriesByTitle(updateDto.Categories, ct);

            product.SetCategories(categories);


            await _productRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Product updated. ProductId: {ProductId}.", productId);

            return product.ProductToProductExtendedDto();
        }



        public async Task<ProductDto?> InactivateProductAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Inactive product. ProductId: {ProductId}", productId);

            Product? product = await _productRepository.FindByIdAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot inactivate. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            product.Deactivate();

            await _productRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Product inactivated. ProductId: {ProductId}.", productId);

            return product.ProductToProductDto();
        }



        public async Task<ProductDto?> ActivateProductAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Activating product. ProductId: {ProductId}", productId);

            Product? product = await _productRepository.FindByIdAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot activate. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            product.Activate();

            await _productRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Product activated. ProductId: {ProductId}.", productId);

            return product.ProductToProductDto();
        }



        public async Task<bool> DeleteProductAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting product. ProductId: {ProductId}.", productId);

            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot delete. Product not foud. ProductId: {ProductId}.", productId);
                return false;
            }


            _productRepository.Remove(product);
            await _productRepository.SaveChangesAsync(ct);
            _logger.LogInformation("Product deleted. ProductId: {ProductId}.", productId);

            return true;
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByBrandIdAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products by BranId. BrandId: {BrandId}.", brandId);

            IReadOnlyList<Product> products = await _productRepository.GetAllProductsByBrandIdAsync(brandId, ct);
            _logger.LogInformation("Retrieved all products by BrandId. Count: {Count}, BrandId: {BrandId}.", products.Count, brandId);

            return [.. products.Select(x => x.ProductToProductDto())];
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByCategoriesAsync(List<string> categories, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products by categories. Categories: {Categories}.", string.Join(", ", categories));

            IReadOnlyList<Product> products = await _productRepository.GetAllProductsByCategoriesAsync(categories, ct);
            _logger.LogInformation("Retrieved all products by categories. Count: {Count}, Categories: {Categories}.", products.Count, string.Join(", ", categories));

            return [.. products.Select(x => x.ProductToProductDto())];
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllActiveProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all active products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllActiveProductsAsync(ct);
            _logger.LogInformation("Retrieved all active products. Count: {Count}.", products.Count);

            return [.. products.Select(x => x.ProductToProductDto())];
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllInactiveProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all inactive products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllInactiveProductsAsync(ct);
            _logger.LogInformation("Retrieved all inactive products. Count: {Count}.", products.Count);

            return [.. products.Select(x => x.ProductToProductDto())];
        }



        public async Task<IReadOnlyList<ProductQuantityCheckResponseDto>> ProductsQuantityCheckFromCartAsync(List<ProductQuantityCheckRequestDto> productsFromCart, CancellationToken ct = default)
        {
            _logger.LogInformation("Checking product availability for productIds: {ProductIds}.", string.Join(", ", productsFromCart.Select(p => p.Id)));

            IReadOnlyList<ProductQuantityCheckResponseDto> productsStock = await _productRepository.GetProductsAsQuantityCheckDtoAsync([.. productsFromCart.Select(p => p.Id)], ct);

            var stockById = productsStock.ToDictionary(p => p.Id);

            List<ProductQuantityCheckResponseDto> outOfStockProducts = [];


            foreach (var cartItem in productsFromCart)
            {
                if (!stockById.TryGetValue(cartItem.Id, out ProductQuantityCheckResponseDto? productFromDb))
                {
                    outOfStockProducts.Add(new() { Id = cartItem.Id, Title = "(Nout found)", QuantityInStock = 0 });

                    continue;
                }

                if (cartItem.Quantity > productFromDb.QuantityInStock)
                {
                    outOfStockProducts.Add(productFromDb);
                }
            }

            return outOfStockProducts;
        }



        public async Task ProductQuantityReserved(List<OrderItemCreatedDto> orderItemsDto, CancellationToken ct = default)
        {
            IReadOnlyList<Product> products = await _productRepository.GetAllProductsByIdsAsync([.. orderItemsDto.Select(o => o.ProductId)], ct);

            var productsInStockById = products.ToDictionary(p => p.Id);

            foreach (var orderItem in orderItemsDto)
            {
                if (productsInStockById.TryGetValue(orderItem.ProductId, out Product? product))
                {
                    product.ReduceStock(orderItem.Quantity);

                }
            }

            await _productRepository.SaveChangesAsync(ct);
        }







    }
}
