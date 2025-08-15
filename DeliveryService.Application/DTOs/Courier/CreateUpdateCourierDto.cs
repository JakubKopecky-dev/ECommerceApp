namespace DeliveryService.Application.DTOs.Courier
{
    public sealed record CreateUpdateCourierDto
    {
        public string Name { get; init; } = "";

        public string? PhoneNumber { get; init; }

        public string? Email { get; init; }
    }
}
