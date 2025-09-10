using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.DTOs.External;

namespace CartService.Application.Common
{
    public readonly record struct CheckoutResult(bool Success, IReadOnlyList<ProductQuantityCheckResponseDto> BadProducts, string? CheckoutUrl);
  
}
