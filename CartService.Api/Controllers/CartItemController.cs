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
        public async Task<IActionResult> CreateCartItemAsync([FromBody] CreateCartItemDto createDto)
        {
            CartItemDto cartItem = await _cartItemService.CreateCartItemAsync(createDto);

            return CreatedAtAction(nameof(GetCartItem), new { cartItemId = cartItem.Id }, cartItem);
        }



        [HttpPatch("{cartItemId}/quantity")]
        public async Task<IActionResult> ChangeCartItemQuantity(Guid cartItemId, [FromBody] ChangeQuantityCartItemDto changeDto)
        {
            CartItemDto? cartItem = await _cartItemService.ChangeCartItemQuantityAsync(cartItemId, changeDto);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(Guid cartItemId)
        {
            CartItemDto? cartItem = await _cartItemService.DeleteCartItemAsync(cartItemId);

            return cartItem is not null ? Ok(cartItem) : NotFound();
        }



    }
}
