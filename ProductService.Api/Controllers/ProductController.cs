using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Services;
using ProductService.Application.Services;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;


        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("all")]
        public async Task<IReadOnlyList<ProductDto>> GetAllProducts(CancellationToken ct) => await _productService.GetAllProductsAsync(ct);



        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(Guid productId, CancellationToken ct)
        {
            ProductExtendedDto? product = await _productService.GetProductByIdAsync(productId, ct);

            return product is not null ? Ok(product) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto, CancellationToken ct)
        {
            ProductExtendedDto product = await _productService.CreateProductAsync(createDto, ct);

            return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product);
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateProductDto updateDto, CancellationToken ct)
        {
            ProductExtendedDto? product = await _productService.UpdateProductAsync(productId, updateDto, ct);

            return product is not null ? Ok(product) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId, CancellationToken ct)
        {
            ProductDto? product = await _productService.DeleteProductAsync(productId, ct);

            return product is not null ? Ok(product) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPatch("{productId}/inactivate")]
        public async Task<IActionResult> InactivateProduct(Guid productId, CancellationToken ct)
        {
            ProductDto? product = await _productService.InactivateProductAsync(productId, ct);

            return product is not null ? Ok(product) : NotFound();
        }



        [Authorize(Roles = UserRoles.Admin)]
        [HttpPatch("{productId}/activate")]
        public async Task<IActionResult> ActivateProduct(Guid productId, CancellationToken ct)
        {
            ProductDto? product = await _productService.ActivateProductAsync(productId, ct);

            return product is not null ? Ok(product) : NotFound();

        }



        [HttpGet("by-brandId/{brandId}")]
        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByBrandId(Guid brandId, CancellationToken ct) => await _productService.GetAllProductsByBrandIdAsync(brandId,ct);



        [HttpGet("by-categories")]
        public async Task<IReadOnlyList<ProductDto>> GetAllProductsByCategories([FromQuery] List<string> categories, CancellationToken ct) => await _productService.GetAllProductsByCategoriesAsync(categories, ct);



        [HttpGet("active")]
        public async Task<IReadOnlyList<ProductDto>> GetAllActiveProducts(CancellationToken ct) => await _productService.GetAllActiveProductsAsync(ct);



        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet("inactive")]
        public async Task<IReadOnlyList<ProductDto>> GetAllInactiveProducts(CancellationToken ct) => await _productService.GetAllInactiveProductsAsync(ct);



    }
}
