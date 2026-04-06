using CartService.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CartService.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity>(CartDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly CartDbContext _dbContext = dbContext;
        protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();



        public async Task<TEntity?> FindByIdAsync(Guid id, CancellationToken ct = default) => await _dbSet.FindAsync([id],ct);



        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) => await _dbSet.ToListAsync(ct);



        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }



        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _dbContext.SaveChangesAsync(ct);
        }



        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }



    }
}
