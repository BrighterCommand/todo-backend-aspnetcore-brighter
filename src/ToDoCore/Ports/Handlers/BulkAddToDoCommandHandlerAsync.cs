using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Model;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class BulkAddToDoCommandHandlerAsync: RequestHandlerAsync<BulkAddToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public BulkAddToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
        public override async Task<BulkAddToDoCommand> HandleAsync(BulkAddToDoCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                foreach (var todo in command.ToDos)
                {
                    var savedItem = await repository.AddAsync(
                        new ToDoItem {Title = todo.Title, Completed = todo.Completed, Order = todo.Order},
                        cancellationToken
                    );

                    command.ToDoItemIds.Add(savedItem.Id);
                }

                return await base.HandleAsync(command, cancellationToken);
            }
        }
    }
}