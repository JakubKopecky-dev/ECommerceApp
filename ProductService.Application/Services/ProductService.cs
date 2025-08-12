using AutoMapper;
using Microsoft.Extensions.Logging;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Repositories;
using ProductService.Application.Interfaces.Services;
using ProductService.Domain.Entity;

namespace ProductService.Application.Services
{
    public class ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IProductReviewRepository productReviewRepository, IMapper mapper, ILogger<ProductService> logger) : IProductService
    {
        private readonly IProductRepository _productRepository = productRepository;
        private readonly ICategoryRepository _categoryRepository = categoryRepository;
        private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ProductService> _logger = logger;



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllAsync(ct);
            _logger.LogInformation("Retrieved all products. Count: {Count}.", products.Count);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }



        public async Task<ProductExtendedDto?> GetProductByIdAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving product. ProductId: {ProductId}.", productId);
            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAsync(productId,ct);
            if (product is null)
            {
                _logger.LogInformation("Product not found. ProductId: {ProductId}.", productId);
                return null;
            }

            _logger.LogInformation("Product found. ProductId: {ProductId}.", productId);

            return _mapper.Map<ProductExtendedDto>(product);
        }



        public async Task<ProductExtendedDto> CreateProductAsync(CreateProductDto createDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating new product. Title: {Title}.", createDto.Title);

            Product product = _mapper.Map<Product>(createDto);
            product.Id = Guid.Empty;
            product.CreatedAt = DateTime.UtcNow;

            List<Category> categories = await _categoryRepository.GetCategoriesByName(createDto.Categories,ct);

            categories.ForEach(product.Categories.Add);

            Product createdProduct = await _productRepository.InsertAsync(product, ct);
            _logger.LogInformation("Product created. ProductId: {ProductId}", createdProduct.Id);

            return _mapper.Map<ProductExtendedDto>(createdProduct);
        }



        public async Task<ProductExtendedDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto, CancellationToken ct = default)
        {
            _logger.LogInformation("Updating product. ProductId: {ProductId}", productId);

            Product? productDb = await _productRepository.FindProductByIdIncludeCategoriesAsync(productId, ct);
            if (productDb is null)
            {
                _logger.LogWarning("Cannot update. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            _mapper.Map<UpdateProductDto, Product>(updateDto, productDb);

            productDb.UpdatedAt = DateTime.UtcNow;

            IReadOnlyList<Category> categories = await _categoryRepository.GetCategoriesByTitle(updateDto.Categories, ct);

            foreach (Category c in productDb.Categories.Except(categories).ToList())
                productDb.Categories.Remove(c);

            foreach (Category c in categories.Except(productDb.Categories).ToList())
                productDb.Categories.Add(c);


            Product updatedProduct = await _productRepository.UpdateAsync(productDb, ct);
            _logger.LogInformation("Product updated. ProductId: {ProductId}.", productId);

            return _mapper.Map<ProductExtendedDto>(updatedProduct);
        }



        public async Task<ProductDto?> InactivateProductAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Inactive product. ProductId: {ProductId}", productId);

            Product? product = await _productRepository.FindByIdAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot inactive. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = false;

            Product updatedProduct = await _productRepository.UpdateAsync(product, ct);
            _logger.LogInformation("Product inactive. ProductId: {ProductId}.",productId);

            return _mapper.Map<ProductDto>(updatedProduct);
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

            product.UpdatedAt = DateTime.UtcNow;
            product.IsActive = true;

            Product updatedProduct = await _productRepository.UpdateAsync(product, ct);
            _logger.LogInformation("Product activated. ProductId: {ProductId}.", productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }



        public async Task<ProductDto?> DeleteProductAsync(Guid productId, CancellationToken ct = default)
        {
            _logger.LogInformation("Deleting product. ProductId: {ProductId}.", productId);

            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAndReviewsAsync(productId, ct);
            if (product is null)
            {
                _logger.LogWarning("Cannot delete. Product not foud. ProductId: {ProductId}.", productId);
                return null;
            }

            ProductDto deletedProduct = _mapper.Map<ProductDto>(product);

            if (product.Reviews.Count > 0)
            {
                _logger.LogInformation("Deleting all related reviews before deleting product. ProductId: {ProductId}, Review count: {Count}.", productId, product.Reviews.Count);
                var deletedTasks = product.Reviews.Select(r => _productReviewRepository.DeleteAsync(r.Id, ct));
                await Task.WhenAll(deletedTasks);
                _logger.LogInformation("All related reviews deleted.");
            }

            _logger.LogInformation("Clearing all related categories before deleting product. ProductId: {ProductId}", productId);

            product.Categories.Clear();
            await _productRepository.UpdateAsync(product, ct);
            _logger.LogInformation("All related categories cleared.");

            await _productRepository.DeleteAsync(productId, ct);
            _logger.LogInformation("Product deleted. ProductId: {ProductId}.", productId);

            return deletedProduct;
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByBrandIdAsync(Guid brandId, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products by BranId. BrandId: {BrandId}.",brandId);

            IReadOnlyList<Product> products = await _productRepository.GetAllProductsByBrandIdAsync(brandId, ct);
            _logger.LogInformation("Retrieved all products by BrandId. Count: {Count}, BrandId: {BrandId}.",products.Count,brandId);

            return _mapper.Map<List<ProductDto>>(products);
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByCategoriesAsync(List<string> categories, CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all products by categories. Categories: {Categories}.", string.Join(", ",categories));

            IReadOnlyList<Product> products = await _productRepository.GetAllProductsByCategoriesAsync(categories, ct);
            _logger.LogInformation("Retrieved all products by categories. Count: {Count}, Categories: {Categories}.", products.Count, string.Join(", ", categories));

            return _mapper.Map<List<ProductDto>>(products); 
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllActiveProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all active products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllActiveProductsAsync(ct);
            _logger.LogInformation("Retrieved all active products. Count: {Count}.", products.Count);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }



        public async Task<IReadOnlyList<ProductDto>> GetAllInactiveProductsAsync(CancellationToken ct = default)
        {
            _logger.LogInformation("Retrieving all inactive products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllInactiveProductsAsync(ct);
            _logger.LogInformation("Retrieved all inactive products. Count: {Count}.", products.Count);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }



    }
}
