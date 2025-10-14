using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;

namespace OrderService.Persistence.Repositories
{
    public abstract class BaseRepository<TEntity>(OrderDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly OrderDbContext _dbContext = dbContext;
        protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();


        public async Task<TEntity?> FindByIdAsync(Guid id, CancellationToken ct = default) => await _dbSet.FindAsync([id],ct);



        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default) => await _dbSet.ToListAsync(ct);



        public async Task<TEntity> InsertAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity,ct);
            await _dbContext.SaveChangesAsync(ct);

            return entity;
        }



        public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken ct = default)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync(ct);

            return entity;
        }



        public async Task DeleteAsync(Guid id,CancellationToken ct = default)
        {
            TEntity? entity = await _dbSet.FindAsync([id], ct);
            if (entity is null)
                return;

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync(ct);
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
