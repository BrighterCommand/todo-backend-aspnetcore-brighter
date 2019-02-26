using System.Threading;
using System.Threading.Tasks;

namespace ToDoCore.Ports.Repositories
{
    public interface IRepositoryAsync<T> where T : IEntity
    {
        Task<T> AddAsync(T newEntity, CancellationToken ct = default(CancellationToken));
        Task DeleteAsync(int toDoId, CancellationToken ct = default(CancellationToken));
        Task DeleteAllAsync(CancellationToken ct = default(CancellationToken));
        Task<T> GetAsync(int entityId, CancellationToken ct = default(CancellationToken));
        Task UpdateAsync(T updatedEntity, CancellationToken ct = default(CancellationToken));
    }
}