using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.DTOs.Cart;
using CartService.Application.DTOs.CartItem;

namespace CartService.Application.Interfaces.Services
{
    public interface ICartService
    {
        Task<bool> CheckoutCartAsync(Guid userId);
        Task<CartDto?> DeleteCartAsync(Guid userId);
        Task<CartExtendedDto> GetOrCreateCartAsync(Guid userId);
    }
}
