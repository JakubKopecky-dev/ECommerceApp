using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using PaymentService.Api.DTOs;
using PaymentService.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using PaymentService.Api.Enum;

namespace PaymentService.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api")]
    public class PaymentController(IPaymentService paymentService) : ControllerBase
    {
        private readonly IPaymentService _paymentService = paymentService;



        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession(CreateCheckoutSessionRequestDto requestDto, CancellationToken ct)
        {
            CreateCheckoutSessionResponseDto response = await _paymentService.CreateCheckoutSessionRequestAsync(requestDto,ct);
            
            return Ok(response);
        }





    }
}
