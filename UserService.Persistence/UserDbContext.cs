using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using UserService.Domain.Common;
using UserService.Infrastructure.Identity;

namespace UserService.Persistence
{
    public class UserDbContext(DbContextOptions<UserDbContext> options, IHostEnvironment env) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventUserLog> AuditEventUserLogs { get; set; }




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

                var auditLog = new AuditEventUserLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = System.Text.Json.JsonSerializer.Serialize(data)
                };
                AuditEventUserLogs.Add(auditLog);
            }
        }



    }
}
