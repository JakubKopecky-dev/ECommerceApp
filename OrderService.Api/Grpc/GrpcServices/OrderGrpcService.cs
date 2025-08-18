using Grpc.Core;
using OrderService.Application.DTOs.Order;
using OrderService.Application.DTOs.OrderItem;
using OrderService.Application.Interfaces.Services;
using OrderService.Grpc;
using OrderGrpc = OrderService.Grpc.OrderService;

namespace OrderService.Api.Grpc.GrpcServices
{
    public class OrderGrpcService(IOrderService orderService) : OrderGrpc.OrderServiceBase
    {
        private readonly IOrderService _orderService = orderService;



        public override async Task<GetOrderByIdResponse> GetOrderById(GetOrderByIdRequest request, ServerCallContext context)
        {
            Guid orderId = Guid.Parse(request.OrderId);

            OrderExtendedDto? order = await _orderService.GetOrderAsync(orderId, context.CancellationToken);

            return order is not null ? new() { UserId = order.UserId.ToString() } : throw new RpcException(new Status(StatusCode.NotFound, "Order not found"));
        }



        public override async Task<CreateOrderFromCartResponse> CreateOrderFromCart(CreateOrderFromCartRequest request, ServerCallContext context)
        {
            ExternalCreateOrderDto createDto = new()
            {
                UserId = Guid.Parse(request.UserId),
                CourierId = Guid.Parse(request.CourierId),
                TotalPrice = decimal.Parse(request.TotalPrice),
                Note = request.Note,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Street = request.Street,
                City = request.City,
                PostalCode = request.PostalCode,
                State = request.State,
                Items = [.. request.Items.Select(i => new ExternalCreateOrderItemDto()
                {
                    ProductId = Guid.Parse(i.ProductId),
                    ProductName = i.ProductName,
                    UnitPrice = decimal.Parse(i.UnitPrice),
                    Quantity = i.Quantity

                })]
            };

            OrderDto? createdOrder = await _orderService.CreateOrderAndDeliveryFromCartAsync(createDto, context.CancellationToken);

            return createdOrder is not null ? new() { OrderId = createdOrder.Id.ToString() } : throw new RpcException(new Status(StatusCode.FailedPrecondition, "Order created but delivery not created"));
        }



    }
}
