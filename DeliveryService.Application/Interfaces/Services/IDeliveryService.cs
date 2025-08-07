using DeliveryService.Application.DTOs.Delivery;

namespace DeliveryService.Application.Interfaces.Services
{
    public interface IDeliveryService
    {
        Task<DeliveryDto?> ChangeDeliveryStatusAsync(Guid deliveryId, ChangeDeliveryStatusDto changeDto);
        Task<DeliveryDto> CreateDeliveryAsync(CreateUpdateDeliveryDto createDto);
        Task<DeliveryDto?> DeleteDeliveryAsync(Guid deliveryId);
        Task<DeliveryExtendedDto?> GetDeliveryByOrderIdAsync(Guid orderId);
        Task<DeliveryDto?> UpdateDeliveryAsync(Guid deliveryId, CreateUpdateDeliveryDto updateDto);
    }
}
