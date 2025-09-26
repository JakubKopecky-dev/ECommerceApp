using CartService.Domain.Common;
using CartService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;

namespace CartService.Persistence
{
    public class CartDbContext(DbContextOptions<CartDbContext> options, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventCartLog> AuditEventCartLogs { get; set; }

        public DbSet<Cart> Carts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Cart>()
                 .HasIndex(c => c.UserId)
                 .IsUnique();


            modelBuilder.Entity<CartItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(10, 2);
        }



        // Audit EF
        public override int SaveChanges()
        {
            AuditEntityChanges();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AuditEntityChanges();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void AuditEntityChanges()
        {
            if (_env.EnvironmentName == "Test")
                return;

            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added
                         || e.State == EntityState.Modified
                         || e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                object data;

                // For updates, capture only the properties that have changed, including their original and current values
                if (entry.State == EntityState.Modified)
                {
                    var changes = new Dictionary<string, object>();
                    foreach (var prop in entry.Properties)
                    {
                        if (!Equals(prop.OriginalValue, prop.CurrentValue))
                        {
                            changes[prop.Metadata.Name] = new
                            {
                                Original = prop.OriginalValue,
                                Current = prop.CurrentValue
                            };
                        }
                    }
                    data = changes;
                }
                else
                {
                    // For inserts and deletes, serialize the entire entity
                    data = entry.Entity;
                }

                var auditLog = new AuditEventCartLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = System.Text.Json.JsonSerializer.Serialize(data)
                };
                AuditEventCartLogs.Add(auditLog);
            }
        }




    }
}
