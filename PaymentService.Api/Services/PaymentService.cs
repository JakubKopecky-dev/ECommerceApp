using Microsoft.Extensions.Logging;
using PaymentService.Api.DTOs;
using PaymentService.Api.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PaymentService.Api.Services
{
    public class PaymentService(ILogger<PaymentService> logger, IConfiguration configuration) :IPaymentService
    {
        private readonly ILogger<PaymentService> _logger = logger;
        private readonly IConfiguration _configuration = configuration;



        public async Task<CreateCheckoutSessionResponseDto> CreateCheckoutSessionRequestAsync(CreateCheckoutSessionRequestDto request, CancellationToken ct = default)
        {
            _logger.LogInformation("Creating checkout session. OrderId: {OrderId}.", request.OrderId);

            List<OrderItemDto> items = request.Items;


            SessionCreateOptions options = new()
            {
                LineItems = [.. items.Select(i => new SessionLineItemOptions
                {
                    PriceData = new()
                    {
                        Currency = "usd",
                        UnitAmount =  (long)(i.UnitPrice *100),
                        ProductData = new() { Name = i.ProductName}
                    },
                Quantity = i.Quantity,

                })],
                Mode = "payment",
                SuccessUrl = _configuration["Stripe:SuccessUrl"],
                CancelUrl = _configuration["Stripe:CancelUrl"],

                ClientReferenceId = request.OrderId.ToString()

            };

            SessionService service = new();
            Session session = await service.CreateAsync(options, cancellationToken: ct);
            _logger.LogInformation("Checkout session created. OrderId: {OrderId}, SessionId: {SessionId}", request.OrderId, session.Id);

            CreateCheckoutSessionResponseDto response = new() { CheckoutUrl = session.Url ?? ""};

            return response;
        }
    }



    
}
