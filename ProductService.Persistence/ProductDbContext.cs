using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Hosting;
using ProductService.Domain.Common;
using ProductService.Domain.Entities;
using System.Text.Json;
using System.Text.Json.Serialization;


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
                .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .ToList();

            foreach (var entry in entries)
            {
                object auditData;

                if (entry.State == EntityState.Modified)
                {
                    // Log only changed properties
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
                    auditData = changes;
                }
                else
                {
                    // Flatten entity data to avoid cycles
                    auditData = entry.Entity switch
                    {
                        Product product => new
                        {
                            product.Id,
                            product.Title,
                            product.Price,
                            product.QuantityInStock,
                            product.IsActive,
                            product.BrandId,
                            Categories = product.Categories?.Select(c => c.Title).ToList(),
                            product.CreatedAt,
                            product.UpdatedAt
                        },

                        Category category => new
                        {
                            category.Id,
                            category.Title,
                            ProductCount = category.Products?.Count ?? 0
                        },

                        Brand brand => new
                        {
                            brand.Id,
                            brand.Title,
                            ProductCount = brand.Products?.Count ?? 0
                        },

                        ProductReview review => new
                        {
                            review.Id,
                            review.ProductId,
                            review.UserId,
                            review.Rating,
                            review.Comment,
                            review.CreatedAt
                        },

                        _ => entry.CurrentValues.ToObject()
                    };
                }

                JsonSerializerOptions jsonSerializerOptions = new()
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = false,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                var jsonOptions = jsonSerializerOptions;

                var auditLog = new AuditEventProductLog
                {
                    EntityName = entry.Metadata.ClrType.Name,
                    EventType = entry.State.ToString(),
                    InsertedDate = DateTime.UtcNow,
                    Data = JsonSerializer.Serialize(auditData, jsonOptions)
                };

                AuditEventProuctLogs.Add(auditLog);
            }
        }




    }
}
