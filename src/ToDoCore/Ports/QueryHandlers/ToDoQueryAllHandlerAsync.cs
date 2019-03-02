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
    public class ToDoQueryAllHandlerAsync : QueryHandlerAsync<ToDoQueryAll, ToDoQueryAll.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoQueryAllHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [QueryLogging(1)]
        [RetryableQuery(2)]
        public override async Task<ToDoQueryAll.Result> ExecuteAsync(ToDoQueryAll request,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_options))
            {
                var items = await uow.ToDoItems
                    //.Skip(request.PageNumber - 1 * request.PageSize)
                    // .Take(request.PageSize)
                    .Select(i => new ToDoByIdQuery.Result(i)).ToListAsync();

                return new ToDoQueryAll.Result(items);
            }
        }
    }
}