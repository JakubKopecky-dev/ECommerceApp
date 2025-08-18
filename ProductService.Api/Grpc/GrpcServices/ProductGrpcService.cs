using Grpc.Core;
using ProductService.Application.DTOs.Product;
using ProductService.Application.Interfaces.Services;
using ProductService.Grpc;
using ProductGrpc = ProductService.Grpc.ProductService;

namespace ProductService.Api.Grpc.GrpcServices
{
    public class ProductGrpcService(IProductService productService) : ProductGrpc.ProductServiceBase
    {
        private readonly IProductService _productService = productService;



        public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
        {
            Guid productId = Guid.Parse(request.ProductId);

            ProductExtendedDto? product = await _productService.GetProductByIdAsync(productId);

            return product is not null ? new() { Title = product.Title, Price = product.Price.ToString(), QuantityInStock = product.QuantityInStock } : throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        public override async Task<ProductsQuantityCheckFromCartResponse> ProductsQuantityCheckFromCart(ProductsQuantityCheckFromCartRequest request, ServerCallContext context)
        {
            List<ProductQuantityCheckRequestDto> productsDto = [.. request.Products.Select(p => new ProductQuantityCheckRequestDto { Id = Guid.Parse(p.Id), Quantity = p.Quantity })];

            IReadOnlyList<ProductQuantityCheckResponseDto> products = await _productService.ProductsQuantityCheckFromCartAsync(productsDto,context.CancellationToken);

            ProductsQuantityCheckFromCartResponse response = new();

            response.Products.AddRange(products.Select(p => new ProductQuantityCheckResponse 
            { Id = p.Id.ToString(),
              Title = p.Title,
              QuantityInStock = p.QuantityInStock 
            }));

            return response;
        }





    }
}
