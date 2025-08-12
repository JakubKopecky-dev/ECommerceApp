using CartService.Application.DTOs.CartItem;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartItemService
    {
        Task<CartItemDto?> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto,CancellationToken ct = default);
        Task<CartItemDto?> CreateCartItemOrChangeQuantityAsync(CreateCartItemDto createDto,CancellationToken ct = default);
        Task<CartItemDto?> DeleteCartItemAsync(Guid cartItemId, CancellationToken ct = default);
        Task<CartItemDto?> GetCartItemAsync(Guid cartItemId, CancellationToken ct = default);
    }
}
