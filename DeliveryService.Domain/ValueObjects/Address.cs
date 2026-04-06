using DeliveryService.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Domain.ValueObjects
{
    public sealed record Address
    {
        public string Street { get; } = null!;

        public string City { get; } = null!;

        public string PostalCode { get; } = null!;

        public string State { get; } = null!;

        private Address() { }

        public Address(string street, string city, string postalCode, string state)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new DomainException("Street is required");

            if (street.Length > 50)
                throw new DomainException("Street is too long");

            if (string.IsNullOrWhiteSpace(city))
                throw new DomainException("City is required");

            if (city.Length > 50)
                throw new DomainException("City is too long");

            if (string.IsNullOrWhiteSpace(postalCode))
                throw new DomainException("PostalCode is required");

            if (postalCode.Length > 10)
                throw new DomainException("PostalCode is too long");

            if (string.IsNullOrWhiteSpace(state))
                throw new DomainException("State is required");

            if (state.Length > 30)
                throw new DomainException("Stete is too long");

            Street = street;
            City = city;
            PostalCode = postalCode;
            State = state;
        }
    }
}
