using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ProductService.Domain.Common;
using ProductService.Domain.Entity;
using Microsoft.Extensions.Hosting;


namespace ProductService.Persistence
{
    public class ProductDbContext(DbContextOptions<ProductDbContext> options, IHostEnvironment env) : DbContext(options)
    {
        private readonly IHostEnvironment _env = env;
        public DbSet<AuditEventProductLog> AuditEventProuctLogs { get; set; }

        public DbSet<Brand> Brands { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProductReview> ProductReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);


            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Title)
                .IsUnique();

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Title)
                .IsUnique();



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

                var auditLog = new AuditEventProductLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = System.Text.Json.JsonSerializer.Serialize(data)
                };
                AuditEventProuctLogs.Add(auditLog);
            }
        }




    }
}
