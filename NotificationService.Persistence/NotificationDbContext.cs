using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entity;

namespace NotificationService.Persistence
{
    public class NotificationDbContext(DbContextOptions<NotificationDbContext> options) : DbContext(options)
    {
        public DbSet<Notification> Notifications { get; set; }

    }
}
