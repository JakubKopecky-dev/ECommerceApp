using DeliveryService.Application.DTOs.Courier;

namespace DeliveryService.Application.Interfaces.Services
{
    public interface ICourierService
    {
        Task<CourierDto> CreateCourierAsync(CreateUpdateCourierDto createDto, CancellationToken ct = default);
        Task<CourierDto?> DeleteCourierAsync(Guid courierId, CancellationToken ct = default);
        Task<IReadOnlyList<CourierDto>> GetAllCouriesAsync(CancellationToken ct = default);
        Task<CourierDto?> GetCourierByIdAsync(Guid courierId, CancellationToken ct = default);
        Task<CourierDto?> UpdateCourierAsync(Guid courierId, CreateUpdateCourierDto updateDto, CancellationToken ct = default);
    }
}
