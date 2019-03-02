using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.CommandHandlers
{
    public class DeleteAllToDosCommandHandlerAsync : RequestHandlerAsync<DeleteAllToDosCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;

        public DeleteAllToDosCommandHandlerAsync(DbContextOptions<ToDoContext> options)
        {
            _options = options;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        [UsePolicyAsync(CommandProcessor.CIRCUITBREAKERASYNC, 2)]
        [UsePolicyAsync(CommandProcessor.RETRYPOLICYASYNC, 3)]
        public override async Task<DeleteAllToDosCommand> HandleAsync(DeleteAllToDosCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                await repository.DeleteAllAsync(cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}