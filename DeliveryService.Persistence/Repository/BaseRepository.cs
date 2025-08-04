using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeliveryService.Application.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Persistence.Repository
{
    public abstract class BaseRepository<TEntity>(DeliveryDbContext dbContext) : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly DeliveryDbContext _dbContext = dbContext;
        protected readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();



        public async Task<TEntity?> FindByIdAsync(Guid id) => await _dbSet.FindAsync(id);



        public async Task<IReadOnlyList<TEntity>> GetAllAsync() => await _dbSet.ToListAsync();



        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }



        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }



        public async Task DeleteAsync(Guid id)
        {
            TEntity? entity = await _dbSet.FindAsync(id);
            if(entity is null)
                return;

            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }



    }
}
