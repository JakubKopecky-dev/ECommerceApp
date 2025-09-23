using OrderService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace OrderService.Api.DependencyInjection
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            db.Database.Migrate();
        }
    }
}
