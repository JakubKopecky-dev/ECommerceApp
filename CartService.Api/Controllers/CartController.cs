using System.Security.Claims;
using CartService.Application.DTOs.Cart;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoles.User)]
    public class CartController(ICartService cartService) : ControllerBase
    {
        private readonly ICartService _cartService = cartService;



        [HttpGet]
        public async Task<IActionResult> GetOrCreateCart(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartExtendedDto cart = await _cartService.GetOrCreateCartByUserIdAsync(userId,ct);

            return Ok(cart);
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteCart(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartDto? cart = await _cartService.DeleteCartByUserIdAsync(userId, ct);

            return cart is not null ? Ok(cart) : NotFound();
        }



        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutCart(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            bool result = await _cartService.CheckoutCartByUserIdAsync(userId, ct);

            return result ? Ok() : BadRequest("Cart is empty or checkout failed.");
        }



    }
}
