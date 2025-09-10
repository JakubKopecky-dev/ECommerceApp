using DeliveryService.Domain.Common;
using DeliveryService.Domain.Enum;

namespace DeliveryService.Domain.Entity
{
    public class Delivery : BaseEntity
    {
        public Guid OrderId { get; set; }

        public Guid CourierId { get; set; }
        public required Courier Courier { get; set; }

        public DeliveryStatus Status { get; set; }

        public DateTime? DeliveredAt { get; set; }

        public string Email { get; init; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Street { get; set; } = "";

        public string City { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string State { get; set; } = "";

        public string? TrackingNumber { get; set; }

    }
}
