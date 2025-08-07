namespace DeliveryService.Application.DTOs.Courier
{
    public class CourierDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = "";

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
