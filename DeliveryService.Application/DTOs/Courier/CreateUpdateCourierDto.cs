using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Application.DTOs.Courier
{
    public sealed record CreateUpdateCourierDto
    {
        public string Name { get; init; } = "";

        [Phone]
        public string? PhoneNumber { get; init; }

        [EmailAddress]
        public string? Email { get; init; }
    }
}
