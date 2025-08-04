using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Domain.Common;
using DeliveryService.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;

namespace DeliveryService.Persistence
{
    public class DeliveryDbContext(DbContextOptions<DeliveryDbContext> options, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventLog> AuditEventLogs { get; set; }

        public DbSet<Courier> Couriers { get; set; }

        public DbSet<Delivery> Deliveries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Courier>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Delivery>()
                .HasIndex(d => d.OrderId);




            //from cascade to restrict
            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(type => type.GetForeignKeys())
                .Where(foreignKey => !foreignKey.IsOwnership && foreignKey.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (IMutableForeignKey foreignKey in cascadeFKs)
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;



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

                // Pokud chceš logovat změny při Update, vypiš změněné vlastnosti s hodnotami před/po
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
                    // Pro přidání/smazání stačí celá entita
                    data = entry.Entity;
                }

                var auditLog = new AuditEventLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = System.Text.Json.JsonSerializer.Serialize(data)
                };
                AuditEventLogs.Add(auditLog);
            }
        }




    }
}
