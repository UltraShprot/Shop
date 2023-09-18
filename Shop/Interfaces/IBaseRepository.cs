namespace Shop.Interfaces
{
    public interface IBaseRepository<T>
    {
        Task Create(T entity);

        IQueryable<T> GetAll();

        Task Update(T entity);

        Task Delete(T entity);

        Task DeleteAll(IQueryable<T> entities);
    }
}
