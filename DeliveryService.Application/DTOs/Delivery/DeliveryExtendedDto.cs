using DeliveryService.Application.DTOs.Courier;
using DeliveryService.Domain.Enum;

namespace DeliveryService.Application.DTOs.Delivery
{
    public sealed record DeliveryExtendedDto
    {
        public Guid Id { get; init; }

        public Guid OrderId { get; init; }

        public required CourierDto Courier { get; init; }

        public DeliveryStatus Status { get; init; }

        public DateTime? DeliveredAt { get; init; }

        public string Street { get; init; } = "";

        public string City { get; init; } = "";

        public string PostalCode { get; init; } = "";

        public string State { get; init; } = "";

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}
