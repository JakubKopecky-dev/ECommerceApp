using OrderService.Application.DTOs.OrderItem;

namespace OrderService.Application.DTOs.Order
{
    public sealed record ExternalCreateOrderDto
    {
        public Guid UserId { get; init; }

        public Guid CourierId { get; init; }

        public decimal TotalPrice { get; init; }

        public string Email { get; init; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Street { get; init; } = "";

        public string City { get; init; } = "";

        public string PostalCode { get; init; } = "";

        public string State { get; init; } = "";

        public string? Note { get; init; }

        public List<ExternalCreateOrderItemDto> Items { get; init; } = [];
    }
}
