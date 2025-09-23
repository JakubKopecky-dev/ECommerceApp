using ProductService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ProductService.Api.DependencyInjection
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
            db.Database.Migrate();
        }
    }
}
