using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using OrderService.Domain.Common;
using OrderService.Domain.Entity;

namespace OrderService.Persistence
{
    public class OrderDbContext(DbContextOptions<OrderDbContext> options, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventLog> AuditEventLogs { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(10, 2);

        }




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
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            var jsonOptions = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                // ReferenceHandler = ReferenceHandler.IgnoreCycles // už není potřeba
            };

            foreach (var entry in entries)
            {
                object payload;

                if (entry.State == EntityState.Modified)
                {
                    var changes = new Dictionary<string, object?>();
                    foreach (var p in entry.Properties)
                    {
                        // jen skalární properties; navigace tady vůbec nejsou
                        if (!Equals(p.OriginalValue, p.CurrentValue))
                        {
                            changes[p.Metadata.Name] = new
                            {
                                Original = p.OriginalValue,
                                Current = p.CurrentValue
                            };
                        }
                    }

                    payload = new
                    {
                        Keys = GetKeys(entry),
                        Changes = changes
                    };
                }
                else if (entry.State == EntityState.Added)
                {
                    payload = new
                    {
                        Keys = GetKeys(entry),
                        Values = GetScalarValues(entry, original: false)
                    };
                }
                else // Deleted
                {
                    payload = new
                    {
                        Keys = GetKeys(entry),
                        Values = GetScalarValues(entry, original: true)
                    };
                }

                AuditEventLogs.Add(new AuditEventLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(payload, jsonOptions)
                });
            }
        }

        // Helpers – jen skalární hodnoty (žádné navigace)
        private static Dictionary<string, object?> GetScalarValues(EntityEntry entry, bool original)
        {
            return entry.Properties
                .Where(p => !p.Metadata.IsShadowProperty() && !p.Metadata.IsIndexerProperty())
                .ToDictionary(
                    p => p.Metadata.Name,
                    p => original ? p.OriginalValue : p.CurrentValue
                );
        }

        private static Dictionary<string, object?> GetKeys(EntityEntry entry)
        {
            return entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
        }











    }
}
