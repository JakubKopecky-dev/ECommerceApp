using DeliveryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Persistence.Configuration.ValueObjects
{
    public static class EmailConfiguration
    {

        public static void ConfigureEmail<T>(this OwnedNavigationBuilder<T, Email> email) where T : class
        {
            email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(100);

        }


    }
}
