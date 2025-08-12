using DeliveryService.Application.DTOs.Delivery;

namespace DeliveryService.Application.Interfaces.Services
{
    public interface IDeliveryService
    {
        Task<DeliveryDto?> ChangeDeliveryStatusAsync(Guid deliveryId, ChangeDeliveryStatusDto changeDto, CancellationToken ct = default);
        Task<DeliveryDto> CreateDeliveryAsync(CreateUpdateDeliveryDto createDto, CancellationToken ct = default);
        Task<DeliveryDto?> DeleteDeliveryAsync(Guid deliveryId, CancellationToken ct = default);
        Task<DeliveryExtendedDto?> GetDeliveryByOrderIdAsync(Guid orderId, CancellationToken ct = default);
        Task<DeliveryDto?> UpdateDeliveryAsync(Guid deliveryId, CreateUpdateDeliveryDto updateDto, CancellationToken ct = default);
    }
}
