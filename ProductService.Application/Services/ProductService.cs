using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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



        public async Task<IReadOnlyList<ProductDto>> GetAllProducts()
        {
            _logger.LogInformation("Retrieving all products.");

            IReadOnlyList<Product> products = await _productRepository.GetAllAsync();
            _logger.LogInformation("Retrieved all products. Count: {Count}.", products.Count);

            return _mapper.Map<IReadOnlyList<ProductDto>>(products);
        }



        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            _logger.LogInformation("Retrieving product. ProductId: {ProductId}.", productId);
            Product? product = await _productRepository.FindByIdAsync(productId);
            if (product is null)
            {
                _logger.LogInformation("Product not found. ProductId: {ProductId}.", productId);
                return null;
            }

            _logger.LogInformation("Product found. ProductId: {ProductId}.", productId);

            return _mapper.Map<ProductDto>(product);
        }



        public async Task<ProductDto> CreateProductAsync(CreateProductDto createDto)
        {
            _logger.LogInformation("Creating product. Title: {Title}.", createDto.Title);

            Product product = _mapper.Map<Product>(createDto);
            product.Id = default;
            product.CreatedAt = DateTime.UtcNow;

            Product createdProduct = await _productRepository.InsertAsync(product);
            _logger.LogInformation("Product created. ProductId: {ProductId}", createdProduct.Id);

            return _mapper.Map<ProductDto>(createdProduct);
        }



        public async Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto)
        {
            _logger.LogInformation("Updating product. ProductId: {ProductId}", productId);

            Product? productDb = await _productRepository.FindProductByIdIncludeCategoriesAsync(productId);
            if (productDb is null)
            {
                _logger.LogWarning("Cannot update. Product not found. ProductId: {ProductId}", productId);
                return null;
            }

            _mapper.Map<UpdateProductDto, Product>(updateDto, productDb);

            productDb.UpdatedAt = DateTime.UtcNow;

            IReadOnlyList<Category> categories = await _categoryRepository.GetCategoriesByTitle(updateDto.Categories);

            foreach (Category c in productDb.Categories.Except(categories).ToList())
                productDb.Categories.Remove(c);

            foreach (Category c in categories.Except(productDb.Categories).ToList())
                productDb.Categories.Add(c);


            Product updatedProduct = await _productRepository.UpdateAsync(productDb);
            _logger.LogInformation("Product updated. ProductId: {ProductId}.", productId);

            return _mapper.Map<ProductDto>(updatedProduct);
        }



        public async Task<ProductDto?> DeleteProductAsync(Guid productId)
        {
            _logger.LogInformation("Deleting product. ProductId: {ProductId}.", productId);

            Product? product = await _productRepository.FindProductByIdIncludeCategoriesAndReviewsAsync(productId);
            if (product is null)
            {
                _logger.LogWarning("Cannot delete. Product not foud. ProductId: {ProductId}.", productId);
                return null;
            }

            ProductDto deletedProduct = _mapper.Map<ProductDto>(product);

            if (product.Reviews.Count > 0)
            {
                _logger.LogInformation("Deleting all related reviews before deleting product. ProductId: {ProductId}, Review count: {Count}.", productId, product.Reviews.Count);
                var deletedTasks = product.Reviews.Select(r => _productReviewRepository.DeleteAsync(r.Id));
                await Task.WhenAll(deletedTasks);
                _logger.LogInformation("All related reviews deleted.");
            }

            _logger.LogInformation("Clearing all related categories before deleting product. ProductId: {ProductId}", productId);

            product.Categories.Clear();
            await _productRepository.UpdateAsync(product);
            _logger.LogInformation("All related categories cleared.");

            await _productRepository.DeleteAsync(productId);
            _logger.LogInformation("Product deleted. ProductId: {ProductId}.", productId);

            return deletedProduct;
        }



    }
}
