using DeliveryService.Domain.Common;

namespace DeliveryService.Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string Name { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public ICollection<Delivery> Deliveries { get; set; } = [];

    }
}
