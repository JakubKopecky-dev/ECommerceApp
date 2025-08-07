namespace OrderService.Application.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task DeleteAsync(Guid id);
        Task<TEntity?> FindByIdAsync(Guid id);
        Task<IReadOnlyList<TEntity>> GetAllAsync();
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
    }
}
