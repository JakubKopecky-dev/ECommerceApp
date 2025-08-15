using CartService.Application.DTOs.CartItem;
using CartService.Application.Common;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartItemService
    {
        Task<Result<CartItemDto, CartItemError>> ChangeCartItemQuantityAsync(Guid cartItemId, ChangeQuantityCartItemDto changeDto,CancellationToken ct = default);
        Task<Result<CartItemDto, CartItemError>> CreateCartItemOrChangeQuantityAsync(CreateCartItemDto createDto,CancellationToken ct = default);
        Task<CartItemDto?> DeleteCartItemAsync(Guid cartItemId, CancellationToken ct = default);
        Task<CartItemDto?> GetCartItemAsync(Guid cartItemId, CancellationToken ct = default);
    }
}
