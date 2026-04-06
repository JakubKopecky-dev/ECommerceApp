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
    public class BrandConfiguration: IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> brand)
        {
            brand.Property(x => x.Title)
                .HasMaxLength(150);

            brand.Property(x => x.Description)
                .HasMaxLength(2000);

            brand.Property(x => x.Id)
                .ValueGeneratedNever();
        }

    }
}
