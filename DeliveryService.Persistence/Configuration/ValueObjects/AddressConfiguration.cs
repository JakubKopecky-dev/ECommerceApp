using DeliveryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Persistence.Configuration.ValueObjects
{
    public static class AddressConfiguration
    {
        public static void ConfigureAddress<T>(this OwnedNavigationBuilder<T, Address> address) where T : class
        {
            address.Property(a => a.Street)
                   .HasMaxLength(50);

            address.Property(a => a.City)
                   .HasMaxLength(50);


            address.Property(a => a.PostalCode)
                   .HasMaxLength(10);


            address.Property(a => a.State)
                   .HasMaxLength(30);

        }
    }
}
