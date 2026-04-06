using DeliveryService.Domain.Common;
using DeliveryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DeliveryService.Persistence
{
    public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options) : DbContext(options)
    {
        public DbSet<Courier> Couriers { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeliveryDbContext).Assembly);
        }




    }
}
