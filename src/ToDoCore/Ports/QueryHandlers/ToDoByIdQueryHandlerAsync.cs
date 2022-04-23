using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Darker;
using Paramore.Darker.Policies;
using Paramore.Darker.QueryLogging;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;

namespace ToDoCore.Ports.QueryHandlers
{
    public class ToDoByIdQueryHandlerAsync : QueryHandlerAsync<ToDoByIdQuery, ToDoByIdQuery.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoByIdQueryHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }


        [QueryLogging(0)]
        [RetryableQuery(1)]
        public override async Task<ToDoByIdQuery.Result> ExecuteAsync(ToDoByIdQuery request, CancellationToken cancellationToken = default)
        {
            await using var uow = new ToDoContext(_options);
            return await uow.ToDoItems.Where(t => t.Id == request.Id).Select(t => new ToDoByIdQuery.Result(t)).SingleOrDefaultAsync(cancellationToken);
        }
    }
}