using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs.Cart
{
    public sealed record CartCheckoutRequestDto
    {
        public Guid CourierId { get; init; }

        public string Email { get; init; } = "";

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string PhoneNumber { get; set; } = "";

        public string Street { get; init; } = "";

        public string City { get; init; } = "";

        public string PostalCode { get; init; } = "";

        public string State { get; init; } = "";

        public string? Note { get; init; } 
    }
}
