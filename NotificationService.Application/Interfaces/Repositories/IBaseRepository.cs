namespace NotificationService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        Task<TEntity?> FindByIdAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default);
    }
}
