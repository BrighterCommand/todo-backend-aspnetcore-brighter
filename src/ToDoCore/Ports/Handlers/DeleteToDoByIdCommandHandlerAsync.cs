using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Handlers
{
    public class DeleteToDoByIdCommandHandlerAsync : RequestHandlerAsync<DeleteToDoByIdCommand>
    {
        private readonly DbContextOptions<ToDoContext> _dbContextOptions;

        public DeleteToDoByIdCommandHandlerAsync(DbContextOptions<ToDoContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
        public override async Task<DeleteToDoByIdCommand> HandleAsync(DeleteToDoByIdCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_dbContextOptions))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                await repository.DeleteAsync(command.ToDoId, cancellationToken);
           }

            return await base.HandleAsync(command, cancellationToken);
       }
    }
}