using GrpcPaymentClient = PaymentService.Grpc.PaymentService.PaymentServiceClient;
using OrderService.Application.Interfaces.External;
using OrderService.Application.DTOs.External;
using PaymentService.Grpc;
using Grpc.Core;

namespace OrderService.Api.Grpc.GrpcClients
{
    public class GrpcPaymentReadClient(GrpcPaymentClient client, ILogger<GrpcPaymentReadClient> logger) : IPaymentReadClient
    {
        private readonly GrpcPaymentClient _client = client;
        private readonly ILogger<GrpcPaymentReadClient> _logger = logger;



        public async Task<CreateCheckoutSessionResponseDto?> CreateCheckoutSessionAsync(CreateCheckoutSessionRequestDto requestDto, CancellationToken ct = default)
        {
            CreateCheckoutSessionRequest request = new()
            {
                OrderId = requestDto.Id.ToString(),
            };

            request.Items.AddRange(requestDto.Items.Select(i => new OrderItems
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice.ToString()
            }));

            try
            {
                CreateCheckoutSessionResponse response = await _client.CreateCheckoutSessionAsync(request, cancellationToken: ct);

                CreateCheckoutSessionResponseDto responseDto = new() { CheckoutUrl = response.CheckoutUrl };

                return responseDto;
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "Payment service call failed with status {StatusCode}", ex.StatusCode);
                return null;
            }
        }


    }
}
