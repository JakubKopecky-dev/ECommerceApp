using CartService.Application.Common;
using CartService.Application.DTOs.Cart;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<Result<CheckoutResult, CartError>> CheckoutCartByUserIdAsync(Guid userId, CartCheckoutRequestDto cartCheckoutRequestDto, CancellationToken ct = default);
        Task<CartDto?> DeleteCartByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<CartExtendedDto> GetOrCreateCartByUserIdAsync(Guid userId, CancellationToken ct = default);
    }
}
