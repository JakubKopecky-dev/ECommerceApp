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
        public async Task<IActionResult> GetOrCreateCart()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartExtendedDto cart = await _cartService.GetOrCreateCartAsync(userId);

            return Ok(cart);
        }



        [HttpDelete]
        public async Task<IActionResult> DeleteCart()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartDto? cart = await _cartService.DeleteCartAsync(userId);

            return cart is not null ? Ok(cart) : NotFound();
        }



        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutCart()
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            bool result = await _cartService.CheckoutCartAsync(userId);

            return result ? Ok() : BadRequest("Cart is empty or checkout failed.");
        }



    }
}
