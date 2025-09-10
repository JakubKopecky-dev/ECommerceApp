using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Events;
using Stripe;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Route("webhook")]
    public class WebhookController(ILogger<WebhookController> logger,IPublishEndpoint publishEndpoint, IConfiguration configuration) : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger = logger;
        private readonly IConfiguration _configuration = configuration;
        private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;



        [HttpPost]
        public async Task<IActionResult> Handle()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var endpointSecret = _configuration["Stripe:WebhookSecret"];

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    endpointSecret
                );

                switch (stripeEvent.Type)
                {
                    case EventTypes.CheckoutSessionCompleted:
                        if (stripeEvent.Data.Object is Stripe.Checkout.Session session)
                        {
                            if (string.IsNullOrEmpty(session.ClientReferenceId))
                            {
                                _logger.LogWarning("Checkout completed, but ClientReferenceId is null for Session {SessionId}", session.Id);
                                return Ok();
                            }


                            if (!Guid.TryParse(session.ClientReferenceId, out var orderId))
                            {
                                _logger.LogWarning("Invalid ClientReferenceId format: {OrderIdString}", session.ClientReferenceId);
                                return Ok();
                            }


                            _logger.LogInformation("Checkout completed for Order {OrderId}, SessionId {SessionId}.", session.ClientReferenceId, session.Id);

                            // Change orderStatus to Paid
                            OrderSuccessfullyPaidAndOrderStatusChangeToPaidEvent changeEvent = new() { OrderId = orderId };
                             await _publishEndpoint.Publish(changeEvent);
                        }
                        break;

                    case EventTypes.PaymentIntentSucceeded:
                        if (stripeEvent.Data.Object is PaymentIntent intent)
                        {
                            _logger.LogInformation("Payment succeeded for {PaymentIntentId}, amount {Amount}", intent.Id, intent.Amount);
                        }
                        break;

                    default:
                        _logger.LogWarning("Unhandled event type: {EventType}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe webhook error");
                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected webhook error");
                return StatusCode(500);
            }
        }







    }
}
