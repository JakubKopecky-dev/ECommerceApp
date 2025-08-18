using System.ComponentModel.DataAnnotations;
using CartService.Application.DTOs.CartItem;

namespace CartService.Application.DTOs.External
{
    public sealed record CreateOrderAndDeliveryDto
    {
        public Guid UserId { get; init; }

        public Guid CourierId { get; init; }

        public decimal TotalPrice { get; init; }

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

        [MaxLength(1000)]
        public string? Note { get; init; }

        public List<CartItemForCheckoutDto> Items { get; init; } = [];
    }
}
