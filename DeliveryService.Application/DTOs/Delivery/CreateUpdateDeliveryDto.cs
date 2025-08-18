using System.ComponentModel.DataAnnotations;

namespace DeliveryService.Application.DTOs.Delivery
{
    public sealed record CreateUpdateDeliveryDto
    {
        public Guid OrderId { get; init; }

        public Guid CourierId { get; init; }

        [EmailAddress]
        public string Email { get; init; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        [Phone]
        public string PhoneNumber { get; set; } = "";

        public string Street { get; init; } = "";

        public string City { get; init; } = "";

        public string PostalCode { get; init; } = "";

        public string State { get; init; } = "";
    }
}
