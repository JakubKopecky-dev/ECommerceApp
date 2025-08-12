using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces.Repositories;
using NotificationService.Domain.Entity;

namespace NotificationService.Persistence.Repository
{
    public class NotificationRepository(NotificationDbContext dbContext) : BaseRepository<Notification>(dbContext), INotificationRepository
    {
        public async Task<IReadOnlyList<Notification>> GetAllNotificationsByUserIdAsync(Guid userId, CancellationToken ct = default) => await _dbSet
            .Where(n => n.UserId == userId)
            .ToListAsync(ct);

    }
}
