using System.Threading;
using System.Threading.Tasks;
using Darker;
using Darker.Attributes;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Queries;

namespace ToDoCore.Ports.Handlers
{
    public class ToDoByIdQueryHandlerAsync : AsyncQueryHandler<ToDoByIdQuery, ToDoByIdQuery.Result>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public ToDoByIdQueryHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [RetryableQuery(2)]
        public override async Task<ToDoByIdQuery.Result> ExecuteAsync(ToDoByIdQuery request, CancellationToken cancellationToken = default(CancellationToken))
        {
           using (var uow = new ToDoContext(_options))
            {
                var toDoItem = await uow.ToDoItems.SingleAsync(t => t.Id == request.Id, cancellationToken: cancellationToken);
                return new ToDoByIdQuery.Result(toDoItem);
            }
        }
    }
}