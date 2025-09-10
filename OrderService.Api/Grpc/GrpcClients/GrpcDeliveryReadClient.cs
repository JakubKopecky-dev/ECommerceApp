using DeliveryService.Grpc;
using Grpc.Core;
using OrderService.Application.DTOs.External;
using OrderService.Application.Interfaces.External;
using GrpcDeliveryClient = DeliveryService.Grpc.DeliveryService.DeliveryServiceClient;
using DeliveryStatusShared = Shared.Contracts.Enums.DeliveryStatus;

namespace OrderService.Api.Grpc.GrpcClients
{
    public class GrpcDeliveryReadClient(GrpcDeliveryClient client) : IDeliveryReadClient
    {
        private readonly GrpcDeliveryClient _client = client;



        public async Task<DeliveryStatusShared?> GetDeliveryStatusByOrderIdAsync(Guid orderId, CancellationToken ct = default)
        {
            try
            {
                GetDeliveryByOrderIdRequest request = new() { OrderId = orderId.ToString() };

                var response = await _client.GetDeliveryByOrderIdAsync(request, cancellationToken: ct);

                return (DeliveryStatusShared)(int)response.Status;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                return null;
            }
        }



        public async Task<Guid> CreateDeliveryAsync(CreateDeliveryDto dto, CancellationToken ct = default)
        {
                CreateDeliveryRequest request = new()
                {
                    OrderId = dto.OrderId.ToString(),
                    CourierId = dto.CourierId.ToString(),
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    PhoneNumber = dto.PhoneNumber,
                    Street = dto.Street,
                    City = dto.City,
                    PostalCode = dto.PostalCode,
                    State = dto.State
                };

                var response = await _client.CreateDeliveryAsync(request, cancellationToken: ct);

                return Guid.Parse(response.DeliveryId);
        }





    }
}
