using UserService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace UserService.Api.DependencyInjection
{
    public static class MigrationApplicationBuilderExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();
            db.Database.Migrate();
        }
    }
}
