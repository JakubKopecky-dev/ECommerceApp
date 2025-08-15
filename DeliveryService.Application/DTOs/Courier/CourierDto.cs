namespace DeliveryService.Application.DTOs.Courier
{
    public sealed record CourierDto
    {
        public Guid Id { get; init; }

        public string Name { get; init; } = "";

        public string? PhoneNumber { get; init; }

        public string? Email { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime? UpdatedAt { get; init; }
    }
}
