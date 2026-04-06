using DeliveryService.Domain.Entities;
using DeliveryService.Persistence.Configuration.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Persistence.Configuration
{
    public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
    {
        public void Configure(EntityTypeBuilder<Delivery> x)
        {
            x.HasKey(d => d.Id);

            x.Property(d => d.Id)
                .ValueGeneratedNever();

            x.HasIndex(d => d.OrderId);

            x.Property(d => d.FirstName)
                .HasMaxLength(20);

            x.Property(d => d.LastName)
                .HasMaxLength(20);

            x.Property(d => d.PhoneNumber)
                .HasMaxLength(20);

            x.Property(d => d.TrackingNumber)
                .HasMaxLength(40);

            x.OwnsOne(d => d.Address, address =>
            {
                address.ConfigureAddress();
            });


            x.OwnsOne(d => d.Email, email =>
            {
                email.ConfigureEmail();
            });
        }




    }
}
