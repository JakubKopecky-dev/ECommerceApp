using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.Common;
using OrderService.Domain.Entities;

namespace OrderService.Persistence
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options) : DbContext(options)
    {
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(o =>
            {
                o.Property(o => o.TotalPrice)
                 .HasPrecision(10, 2);

                o.Property(o => o.Note)
                 .HasMaxLength(1000);

                o.Property(o => o.Id)
                 .ValueGeneratedNever();
            });
                

            modelBuilder.Entity<OrderItem>(x =>
            {
                x.Property(oi => oi.UnitPrice)
                 .HasPrecision(10, 2);

                x.Property(oi => oi.ProductName)
                 .HasMaxLength(150);

                x.Property(oi => oi.Id)
                 .ValueGeneratedNever();
            });



        }


    }
}
