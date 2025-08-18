using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CartService.Application.DTOs.External;

namespace CartService.Application.Interfaces.External
{
    public interface IProductReadClient
    {
        Task<IReadOnlyList<ProductQuantityCheckResponseDto>> CheckProductAvailabilityAsync(List<ProductQuantityCheckRequestDto> requestDto, CancellationToken ct = default);
        Task<ProductDto?> GetProductAsync(Guid productId, CancellationToken ct = default);
    }
}
