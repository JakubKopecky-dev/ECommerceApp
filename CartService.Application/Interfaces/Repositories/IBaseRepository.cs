namespace CartService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task DeleteAsync(Guid id);
        Task<TEntity?> FindByIdAsync(Guid id);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}
