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



        [HttpGet("{cartItemId}")]
        public async Task<IActionResult> GetCartItem(Guid cartItemId)
        {
            CartItemDto? cartItem = await _cartItemService.GetCartItemAsync(cartItemId);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> CreateCartItemAsync([FromBody] CreateCartItemDto createDto, CancellationToken ct)
        {
            var result = await _cartItemService.CreateCartItemOrChangeQuantityAsync(createDto, ct);

            return result.ToCreatedAtActionResult(this, nameof(GetCartItem), new { cartItemId = result.Value?.Id });
        }



        [HttpPatch("{cartItemId}/quantity")]
        public async Task<IActionResult> ChangeCartItemQuantity(Guid cartItemId, [FromBody] ChangeQuantityCartItemDto changeDto, CancellationToken ct)
        {
            var result = await _cartItemService.ChangeCartItemQuantityAsync(cartItemId, changeDto, ct);

            return result.ToActionResult(this);
        }



        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            CartItemDto? cartItem = await _cartItemService.DeleteCartItemAsync(cartItemId);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



    }
}
