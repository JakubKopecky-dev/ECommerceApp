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
    public class OrderDbContext(DbContextOptions<OrderDbContext> options, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventOrderLog> AuditEventOrderLogs { get; set; }

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
            };

            foreach (var entry in entries)
            {
                object data;

                if (entry.State == EntityState.Modified)
                {
                    var changes = new Dictionary<string, object?>();
                    foreach (var p in entry.Properties)
                    {
                        // Only include scalar properties; navigation properties are not tracked here
                        if (!Equals(p.OriginalValue, p.CurrentValue))
                        {
                            changes[p.Metadata.Name] = new
                            {
                                Original = p.OriginalValue,
                                Current = p.CurrentValue
                            };
                        }
                    }

                    data = new
                    {
                        Keys = GetKeys(entry),
                        Changes = changes
                    };
                }
                else if (entry.State == EntityState.Added)
                {
                    data = new
                    {
                        Keys = GetKeys(entry),
                        Values = GetScalarValues(entry, original: false)
                    };
                }
                else // Deleted
                {
                    data = new
                    {
                        Keys = GetKeys(entry),
                        Values = GetScalarValues(entry, original: true)
                    };
                }

                AuditEventOrderLogs.Add(new AuditEventOrderLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(data, jsonOptions)
                });
            }
        }

        // Helpers – extract only scalar values (exclude navigation properties)
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
