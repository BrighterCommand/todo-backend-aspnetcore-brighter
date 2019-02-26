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
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.Handlers
{
    public class AddToDoCommandHandlerAsync : RequestHandlerAsync<AddToDoCommand>
    {
        private readonly DbContextOptions<ToDoContext> _options;
        private readonly IAmACommandProcessor _commandProcessor;

        public AddToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options, IAmACommandProcessor commandProcessor)
        {
            _options = options;
            _commandProcessor = commandProcessor;
        }

        [RequestLoggingAsync(step: 1, timing: HandlerTiming.Before)]
        [UsePolicyAsync(policy: CommandProcessor.CIRCUITBREAKERASYNC, step:2)]
        [UsePolicyAsync(policy: CommandProcessor.RETRYPOLICYASYNC, step: 3)]
        public override async Task<AddToDoCommand> HandleAsync(AddToDoCommand command, CancellationToken cancellationToken = new CancellationToken())
        {
            using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                var savedItem = await repository.AddAsync(
                    new ToDoItem {Title = command.Title, Completed = command.Commpleted, Order = command.Order},
                    cancellationToken
                );
                command.ToDoItemId = savedItem.Id;
            }

            _commandProcessor.Post(new TaskCreatedEvent(command.Title));
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}