using CartService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CartService.Api.DependencyInjection
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CartDbContext>();
            db.Database.Migrate();
        }
    }
}
