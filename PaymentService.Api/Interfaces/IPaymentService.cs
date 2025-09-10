using PaymentService.Api.DTOs;

namespace PaymentService.Api.Interfaces
{
    public interface IPaymentService
    {
        Task<CreateCheckoutSessionResponseDto> CreateCheckoutSessionRequestAsync(CreateCheckoutSessionRequestDto request, CancellationToken ct = default);
    }
}
