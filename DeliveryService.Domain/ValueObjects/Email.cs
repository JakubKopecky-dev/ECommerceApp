using DeliveryService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Domain.ValueObjects
{
    public sealed record Email
    {
        public string Value { get; } = null!;


        private Email() { }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("Email is required");

            if (!value.Contains('@'))
                throw new DomainException("Invalid email");

            if (value.Length > 100)
                throw new DomainException("Email is too long");

            Value = value;
        }


        public override string ToString() => Value;


    }
}
