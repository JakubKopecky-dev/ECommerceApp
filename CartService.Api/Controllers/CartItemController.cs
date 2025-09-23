using CartService.Api.Extensions;
using CartService.Application.Common;
using CartService.Application.DTOs.CartItem;
using CartService.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartItemController(ICartItemService cartItemService) : ControllerBase
    {
        private readonly ICartItemService _cartItemService = cartItemService;



        /// <summary>
        /// Retrieves the details of a specific cart item by its unique identifier.
        /// </summary>
        /// <remarks>Returns an HTTP 200 response with the cart item data if the item exists, or an HTTP
        /// 404 response if no item with the specified identifier is found.</remarks>
        /// <param name="cartItemId">The unique identifier of the cart item to retrieve.</param>
        /// <returns>An <see cref="IActionResult"/> containing the cart item details if found; otherwise, a NotFound result.</returns>
        [HttpGet("{cartItemId}")]
        public async Task<IActionResult> GetCartItem(Guid cartItemId)
        {
            CartItemDto? cartItem = await _cartItemService.GetCartItemByIdAsync(cartItemId);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



        /// <summary>
        /// Creates a new cart item or updates the quantity of an existing item in the cart.
        /// </summary>
        /// <param name="createDto">The data transfer object containing the details of the cart item to create or update. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An IActionResult that represents the result of the operation. Returns a CreatedAtAction result with the
        /// details of the created or updated cart item.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateCartItem([FromBody] CreateCartItemDto createDto, CancellationToken ct)
        {
            var result = await _cartItemService.CreateCartItemOrChangeQuantityAsync(createDto, ct);

            return result.ToCreatedAtActionResult(this, nameof(GetCartItem), new { cartItemId = result.Value?.Id });
        }



        /// <summary>
        /// Updates the quantity of a specific item in the shopping cart.
        /// </summary>
        /// <param name="cartItemId">The unique identifier of the cart item to update.</param>
        /// <param name="changeDto">An object containing the new quantity for the cart item. Cannot be null.</param>
        /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An <see cref="IActionResult"/> indicating the result of the update operation. Returns <see cref="OkResult"/>
        /// if the update is successful, or an appropriate error result if the operation fails.</returns>
        [HttpPatch("{cartItemId}/quantity")]
        public async Task<IActionResult> ChangeCartItemQuantity(Guid cartItemId, [FromBody] ChangeQuantityCartItemDto changeDto, CancellationToken ct)
        {
            var result = await _cartItemService.ChangeCartItemQuantityAsync(cartItemId, changeDto, ct);

            return result.ToActionResult(this);
        }



        /// <summary>
        /// Deletes the specified cart item from the user's shopping cart.
        /// </summary>
        /// <param name="cartItemId">The unique identifier of the cart item to delete.</param>
        /// <returns>An <see cref="OkObjectResult"/> containing the deleted cart item if the operation succeeds; otherwise, a
        /// <see cref="NotFoundResult"/> if the cart item does not exist.</returns>
        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            CartItemDto? cartItem = await _cartItemService.DeleteCartItemAsync(cartItemId);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



    }
}
