using CartService.Application.DTOs.External;
using CartService.Application.Interfaces.External;
using Grpc.Core;
using OrderService.Grpc;
using GrpcOrderClient = OrderService.Grpc.OrderService.OrderServiceClient;

namespace CartService.Api.Grpc.GrpcClients
{
    public class GrpcOrderReadClient(GrpcOrderClient client) : IOrderReadClient
    {
        private readonly GrpcOrderClient _client = client;


        public async Task<Guid?> CreateOrderAndDeliveryAsync(CreateOrderAndDeliveryDto checkOutCartDto, CancellationToken ct = default)
        {
            CreateOrderFromCartRequest request = new()
            {
                UserId = checkOutCartDto.UserId.ToString(),
                CourierId = checkOutCartDto.UserId.ToString(),
                TotalPrice = checkOutCartDto.TotalPrice.ToString(),
                Note = checkOutCartDto.Note,
                Email = checkOutCartDto.Email,
                FirstName = checkOutCartDto.FirstName,
                LastName = checkOutCartDto.LastName,
                PhoneNumber = checkOutCartDto.PhoneNumber,
                Street = checkOutCartDto.Street,
                City = checkOutCartDto.City,
                PostalCode = checkOutCartDto.PostalCode,
                State = checkOutCartDto.State,

            };

            request.Items.AddRange(checkOutCartDto.Items.Select(i => new CartItemForCheckout
            {
                ProductId = i.ProductId.ToString(),
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice.ToString(),
                Quantity = i.Quantity
            }));

            try
            {
                var response = await _client.CreateOrderFromCartAsync(request, cancellationToken: ct);

                return Guid.Parse(response.OrderId);
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.FailedPrecondition)
            {
                return null;
            }
        }



    }
}
