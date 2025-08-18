using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using Grpc.Core;
using ProductService.Grpc;
using GprcProductClient = ProductService.Grpc.ProductService.ProductServiceClient;

namespace CartService.Api.Grpc.GrpcClients
{
    public class GrpcProductReadClient(GprcProductClient client) : IProductReadClient
    {
        private readonly GprcProductClient _client = client;



        public async Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken ct = default)
        {
            try
            {
                GetProductByIdRequest request = new() { ProductId = productId.ToString() };
                var response = await _client.GetProductByIdAsync(request, cancellationToken: ct);

                ProductDto product = new() { Title = response.Title, Price = decimal.Parse(response.Price), QuantityInStock = response.QuantityInStock };

                return product;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }



        public async Task<IReadOnlyList<ProductQuantityCheckResponseDto>> CheckProductAvailabilityAsync(List<ProductQuantityCheckRequestDto> requestDto, CancellationToken ct = default)
        {
            ProductsQuantityCheckFromCartRequest request = new();
            request.Products.AddRange(requestDto.Select(r => new ProductQuantityCheckRequest
            {
                Id = r.Id.ToString(),
                Quantity = r.Quantity,
            }));

            var response = await _client.ProductsQuantityCheckFromCartAsync(request, cancellationToken: ct);

            IReadOnlyList<ProductQuantityCheckResponseDto> badProducts = [.. response.Products.Select(p => new ProductQuantityCheckResponseDto
            { Id = Guid.Parse(p.Id),
              Title = p.Title,
              QuantityInStock = p.QuantityInStock,
            })];

            return badProducts;
        }



    }
}
