using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using ProductService.Domain.Common;
using ProductService.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace ProductService.Persistence
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options) : DbContext(options)
    {
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);


            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Title)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Title)
                .IsUnique();


        }





    }
}
