using CartService.Application.DTOs.Cart;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<bool> CheckoutCartAsync(Guid userId);
        Task<CartDto?> DeleteCartAsync(Guid userId);
        Task<CartExtendedDto> GetOrCreateCartAsync(Guid userId);
    }
}
