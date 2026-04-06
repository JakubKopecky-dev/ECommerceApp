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
    public class ProductReviewConfiguration: IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> review)
        {
            review.Property(x => x.Id)
                .ValueGeneratedNever();

            review.Property(x => x.Title)
                .HasMaxLength(150);

            review.Property(x => x.Comment)
                .HasMaxLength(1000);

            review.Property(x => x.Rating)
                .HasMaxLength(5);
        }
    }
}
