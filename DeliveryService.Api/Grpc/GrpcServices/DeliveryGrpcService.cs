using DeliveryService.Application.DTOs.Delivery;
using DeliveryService.Application.Interfaces.Services;
using DeliveryService.Grpc;
using Grpc.Core;
using DeliveryGrpc = DeliveryService.Grpc.DeliveryService;

namespace DeliveryService.Api.Grpc.GrpcServices
{
    public class DeliveryGrpcService(IDeliveryService deliveryService) : DeliveryGrpc.DeliveryServiceBase
    {
        private readonly IDeliveryService _deliveryService = deliveryService;


        public override async Task<GetDeliveryByOrderIdResponse> GetDeliveryByOrderId(GetDeliveryByOrderIdRequest request, ServerCallContext context)
        {
            if (Guid.TryParse(request.OrderId, out var orderId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid order_id GUID."));

            DeliveryExtendedDto? delivery = await _deliveryService.GetDeliveryByOrderIdAsync(orderId, context.CancellationToken);

            return delivery is not null ? new() { Status = (DeliveryService.Grpc.DeliveryStatus)delivery.Status } : throw new RpcException(new Status(StatusCode.NotFound, "Delivery not found."));
        }



        public override async Task<CreateDeliveryResponse> CreateDelivery(CreateDeliveryRequest request, ServerCallContext context)
        {
            CreateUpdateDeliveryDto createDto = new()
            {
                OrderId = Guid.Parse(request.OrderId),
                CourierId = Guid.Parse(request.CourierId),
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Street = request.Street,
                City = request.City,
                PostalCode = request.PostalCode,
                State = request.State,
            };

            DeliveryDto createdDelivery = await _deliveryService.CreateDeliveryAsync(createDto, context.CancellationToken);

            CreateDeliveryResponse response = new() { DeliveryId = createdDelivery.Id.ToString() };

            return response;
        }





    }
}
