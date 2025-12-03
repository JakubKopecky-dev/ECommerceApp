using System.Security.Claims;
using CartService.Api.Extensions;
using CartService.Application.DTOs.Cart;
using CartService.Application.Interfaces.Services;
using CartService.Domain.Enums;
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

        

        /// <summary>
        /// Retrieves the current user's shopping cart or creates a new cart if one does not exist.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> containing the user's cart data if the user is authenticated; otherwise, an
        /// unauthorized result.</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrCreateCart(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartExtendedDto cart = await _cartService.GetOrCreateCartByUserIdAsync(userId,ct);

            return Ok(cart);
        }



        /// <summary>
        /// Deletes the current user's shopping cart.
        /// </summary>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="OkObjectResult"/> containing the deleted cart if the operation succeeds; <see
        /// cref="NotFoundResult"/> if no cart exists for the user; or <see cref="UnauthorizedResult"/> if the user is
        /// not authenticated.</returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCart(CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            CartDto? cart = await _cartService.DeleteCartByUserIdAsync(userId, ct);

            return cart is not null ? Ok(cart) : NotFound();
        }




        /// <summary>
        /// Processes the checkout operation for the current user's shopping cart.
        /// </summary>
        /// <remarks>This method requires the user to be authenticated. The checkout process is performed
        /// asynchronously and may involve payment processing and order creation.</remarks>
        /// <param name="cartCheckoutRequestDto">The details of the cart checkout, including payment and shipping information.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the checkout operation.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the checkout operation. Returns an unauthorized
        /// result if the user is not authenticated.</returns>
        [HttpPost("checkout")]
        public async Task<IActionResult> CheckoutCart(CartCheckoutRequestDto cartCheckoutRequestDto,CancellationToken ct)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var result = await _cartService.CheckoutCartByUserIdAsync(userId,cartCheckoutRequestDto, ct);

            return result.ToActionResult(this);
        }



    }
}
