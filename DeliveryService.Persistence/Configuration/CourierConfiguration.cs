using DeliveryService.Domain.Entities;
using DeliveryService.Persistence.Configuration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryService.Persistence.Configuration
{
    public class CourierConfiguration : IEntityTypeConfiguration<Courier>
    {

        public void Configure(EntityTypeBuilder<Courier> x)
        {
            x.HasIndex(c => c.Name)
                .IsUnique();

            x.Property(c => c.Name).HasMaxLength(30);

            x.OwnsOne(x => x.Email, email =>
            {
                email.ConfigureEmail();
            });

            x.Property(c => c.PhoneNumber).HasMaxLength(20);

            x.Property(c => c.Id)
            .ValueGeneratedNever();



        }

        
    }
}
