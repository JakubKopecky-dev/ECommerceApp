using CartService.Application.DTOs.CartItem;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartItemService
    {
        Task<CartItemDto?> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto);
        Task<CartItemDto> CreateCartItemAsync(CreateCartItemDto createDto);
        Task<CartItemDto?> DeleteCartItemAsync(Guid cartItemId);
        Task<CartItemDto?> GetCartItemAsync(Guid cartItemId);
    }
}
