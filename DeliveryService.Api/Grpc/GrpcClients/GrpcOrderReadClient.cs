using DeliveryService.Application.Interfaces.External;
using Grpc.Core;
using OrderService.Grpc;
using GrpcOrderClient = OrderService.Grpc.OrderService.OrderServiceClient;

namespace DeliveryService.Api.Grpc.GrpcClients
{
    public class GrpcOrderReadClient(GrpcOrderClient client) : IOrderReadClient
    {
        private readonly GrpcOrderClient _client =  client;



        public async Task<Guid?> GetUserIdByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            try
            {
                GetOrderByIdRequest request = new() { OrderId = orderId.ToString() };

                var response = await _client.GetOrderByIdAsync(request,cancellationToken: ct);

                return Guid.TryParse(response.UserId, out var userId) ? userId : null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }



    }
}
