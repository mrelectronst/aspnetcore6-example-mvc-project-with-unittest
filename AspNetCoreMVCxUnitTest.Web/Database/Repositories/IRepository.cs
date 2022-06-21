namespace AspNetCoreMVCxUnitTest.Web.Database.Repositories
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync();

        Task<TEntity> GetByIdAsync(Guid Id);

        Task CreateAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(TEntity entity);
    }
}
