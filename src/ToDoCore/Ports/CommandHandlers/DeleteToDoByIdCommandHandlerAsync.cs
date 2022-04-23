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
    public class DeleteToDoByIdCommandHandlerAsync : RequestHandlerAsync<DeleteToDoByIdCommand>
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;

        public DeleteToDoByIdCommandHandlerAsync(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        [UsePolicyAsync(CommandProcessor.CIRCUITBREAKERASYNC, 2)]
        [UsePolicyAsync(CommandProcessor.RETRYPOLICYASYNC, 3)]
        public override async Task<DeleteToDoByIdCommand> HandleAsync(DeleteToDoByIdCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await using (var uow = new ToDoContext(_dbContextOptions))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                await repository.DeleteAsync(command.ToDoId, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}