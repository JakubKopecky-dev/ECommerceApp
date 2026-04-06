using ProductService.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.ValueObject
{
    public sealed record ImageUrl
    {
        public string Value { get; }

        public ImageUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainException("ImageUrl is required");

            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                throw new DomainException("ImageUrl is not valid");

            Value = value;
        }
    }

}
