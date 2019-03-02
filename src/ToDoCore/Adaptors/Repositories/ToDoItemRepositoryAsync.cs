using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Repositories;

namespace ToDoCore.Adaptors.Repositories
{
    public class ToDoItemRepositoryAsync : IRepositoryAsync<ToDoItem>
    {
        private readonly ToDoContext _uow;

        public ToDoItemRepositoryAsync(ToDoContext uow)
        {
            _uow = uow;
        }

        public async Task<ToDoItem> AddAsync(ToDoItem newEntity, CancellationToken ct = default(CancellationToken))
        {
            var savedItem = _uow.ToDoItems.Add(newEntity);
            await _uow.SaveChangesAsync(ct);
            return savedItem.Entity;
        }

        public async Task DeleteAsync(int toDoId, CancellationToken ct = default(CancellationToken))
        {
            var toDoItem = await _uow.ToDoItems.SingleAsync(t => t.Id == toDoId, ct);
            _uow.Remove(toDoItem);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAllAsync(CancellationToken ct = default(CancellationToken))
        {
            _uow.ToDoItems.RemoveRange(await _uow.ToDoItems.ToListAsync(ct));
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<ToDoItem> GetAsync(int entityId, CancellationToken ct = new CancellationToken())
        {
            return await _uow.ToDoItems.SingleAsync(t => t.Id == entityId, ct);
        }

        public async Task UpdateAsync(ToDoItem updatedEntity, CancellationToken ct = new CancellationToken())
        {
            await _uow.SaveChangesAsync(ct);
        }
    }
}