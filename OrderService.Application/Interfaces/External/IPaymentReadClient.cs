using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderService.Application.DTOs.External;

namespace OrderService.Application.Interfaces.External
{
    public interface IPaymentReadClient
    {
        Task<CreateCheckoutSessionResponseDto?> CreateCheckoutSessionAsync(CreateCheckoutSessionRequestDto requestDto, CancellationToken ct = default);
    }
}
