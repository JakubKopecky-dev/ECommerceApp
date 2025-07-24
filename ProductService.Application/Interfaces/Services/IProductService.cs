using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductService.Application.DTOs.Product;

namespace ProductService.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<ProductDto?> ActivateProductAsync(Guid productId);
        Task<ProductDto> CreateProductAsync(CreateProductDto createDto);
        Task<ProductDto?> DeleteProductAsync(Guid productId);
        Task<IReadOnlyList<ProductDto>> GetAllProducts();
        Task<ProductDto?> GetProductByIdAsync(Guid productId);
        Task<ProductDto?> InactivateProductAsync(Guid productId);
        Task<ProductDto?> UpdateProductAsync(Guid productId, UpdateProductDto updateDto);
    }
}
