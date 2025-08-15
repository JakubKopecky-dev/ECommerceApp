using DeliveryService.Domain.Enum;

namespace DeliveryService.Application.DTOs.Delivery
{
    public sealed record ChangeDeliveryStatusDto
    {
        public DeliveryStatus Status { get; init; }

    }
}
