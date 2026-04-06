using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProductService.Persistence.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {

        public void Configure(EntityTypeBuilder<Product> product)
        {
            product.Property(x => x.Title)
                 .HasMaxLength(150);

            product.Property(p => p.Price)
                .HasPrecision(10, 2);

            product.Property(x => x.Id)
                .ValueGeneratedNever();

            product.OwnsOne(p => p.ImageUrl, img =>
            {
                img.Property(i => i.Value)
                    .HasColumnName("ImageUrl")
                    .HasMaxLength(500);
            });


        }
    }
}
