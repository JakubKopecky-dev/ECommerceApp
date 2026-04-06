using CartService.Domain.Common;
using CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;

namespace CartService.Persistence
{
    public class CartDbContext(DbContextOptions<CartDbContext> options) : DbContext(options)
    {

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Cart>(c =>
            {
                c.HasIndex(c => c.UserId)
                 .IsUnique();

                c.Property(c => c.Id)
                 .ValueGeneratedNever();
            });


            modelBuilder.Entity<CartItem>(c =>
            {
                c.Property(i => i.UnitPrice)
                .HasPrecision(10, 2);


                c.Property(i => i.ProductName)
                .HasMaxLength(150)
                .IsRequired();


                c.Property(i => i.Id)
                .ValueGeneratedNever();
            });

        }







    }
}
