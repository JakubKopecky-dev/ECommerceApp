using DeliveryService.Application.DTOs.Courier;

namespace DeliveryService.Application.Interfaces.Services
{
    public interface ICourierService
    {
        Task<CourierDto> CreateCourierAsync(CreateUpdateCourierDto createDto);
        Task<CourierDto?> DeleteCourierAsync(Guid courierId);
        Task<IReadOnlyList<CourierDto>> GetAllCouriesAsync();
        Task<CourierDto?> GetCourierAsync(Guid courierId);
        Task<CourierDto?> UpdateCourierAsync(Guid courierId, CreateUpdateCourierDto updateDto);
    }
}
