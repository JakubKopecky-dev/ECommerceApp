using Grpc.Core;
using PaymentService.Api.DTOs;
using PaymentService.Api.Interfaces;
using PaymentService.Grpc;
using Stripe;
using PaymentGrpc = PaymentService.Grpc.PaymentService;

namespace PaymentService.Api.Grpc.GrpcService
{
    public class PaymentGrpcService(IPaymentService paymentService) : PaymentGrpc.PaymentServiceBase
    {
        private readonly IPaymentService _paymentService = paymentService;



        public override async Task<CreateCheckoutSessionResponse> CreateCheckoutSession(CreateCheckoutSessionRequest request, ServerCallContext context)
        {
            try
            {
                CreateCheckoutSessionRequestDto requestDto = new()
                {
                    OrderId = Guid.Parse(request.OrderId),
                    Items = [.. request.Items.Select(i => new OrderItemDto
                    { 
                      ProductName = i.ProductName,
                      Quantity = i.Quantity,
                      UnitPrice = decimal.Parse(i.UnitPrice)
                    }
                    )],
                };

                CreateCheckoutSessionResponseDto responseDto = await _paymentService.CreateCheckoutSessionRequestAsync(requestDto, context.CancellationToken);

                CreateCheckoutSessionResponse response = new() { CheckoutUrl = responseDto.CheckoutUrl };

                return response;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"Error while creating checkout session: {ex.Message}"));
            }
        }

    }
}
