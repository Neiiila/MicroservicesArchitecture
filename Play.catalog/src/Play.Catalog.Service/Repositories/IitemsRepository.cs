using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Repositories
{
    public interface IRepository<T> where T : IEntity
    {
        Task CreateAsync(T item);
        Task DeleteAsync(Guid id);
        Task<T> GetAsync(Guid id);
        Task<IReadOnlyCollection<T>> GetAsync();
        Task UpdateAsync(T item);
    }

}