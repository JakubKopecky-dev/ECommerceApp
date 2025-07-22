using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Services;

namespace ProductService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;



        [HttpGet]
        public async Task<IReadOnlyList<ProductDto>> GetAllProducts() => await _productService.GetAllProducts();



        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProduct(Guid productId)
        {
            ProductDto? product = await _productService.GetProductByIdAsync(productId);

            return product is not null ? Ok(product) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createDto)
        {
            ProductDto product = await _productService.CreateProductAsync(createDto);

            return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product);
        }



        [HttpPut("{productId}")]
        public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] UpdateProductDto updateDto)
        {
            ProductDto? product = await _productService.UpdateProductAsync(productId, updateDto);

            return product is not null ? Ok(product) : NotFound();
        }



        [HttpDelete("{productId}")]
        public async Task<IActionResult> DeleteProduct(Guid productId)
        {
            ProductDto? product = await _productService.DeleteProductAsync(productId);

            return product is not null ? Ok(product) : NotFound();
        }



    }
}
