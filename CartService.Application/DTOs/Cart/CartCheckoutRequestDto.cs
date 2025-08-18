using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.DTOs.Cart
{
    public sealed record CartCheckoutRequestDto
    {
        public Guid CourierId { get; init; }

        [EmailAddress]
        public string Email { get; init; } = "";

        [MinLength(2)]
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
    }
}
