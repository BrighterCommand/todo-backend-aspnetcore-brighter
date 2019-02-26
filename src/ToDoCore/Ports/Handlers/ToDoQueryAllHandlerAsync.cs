using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using Microsoft.EntityFrameworkCore;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;

namespace ToDoCore.Ports.Handlers
{
    public class ToDoQueryAllHandlerAsync : AsyncQueryHandler<ToDoQueryAll, ToDoQueryAll.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoQueryAllHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [RequestLogging(1)]
        [RetryableQuery(2)]
        public override async Task<ToDoQueryAll.Result> ExecuteAsync(ToDoQueryAll request, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_options))
            {
                var items = await uow.ToDoItems
                    //.Skip(request.PageNumber - 1 * request.PageSize)
                   // .Take(request.PageSize)
                    .ToArrayAsync(cancellationToken: cancellationToken);

                var todos = new ToDoByIdQuery.Result[items.Length];
                for (var i = 0; i < items.Length; i++)
                {
                    todos[i] = new ToDoByIdQuery.Result(items[i]);
                }
                return new ToDoQueryAll.Result(todos);
            }
        }
    }
}