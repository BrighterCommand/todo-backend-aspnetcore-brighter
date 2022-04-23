using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Domain;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.CommandHandlers
{
    public class AddToDoCommandHandlerAsync : RequestHandlerAsync<AddToDoCommand>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly DbContextOptions<ToDoContext> _options;

        public AddToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options, IAmACommandProcessor commandProcessor)
        {
            _options = options;
            _commandProcessor = commandProcessor;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        [UsePolicyAsync(CommandProcessor.CIRCUITBREAKERASYNC, 2)]
        [UsePolicyAsync(CommandProcessor.RETRYPOLICYASYNC, 3)]
        public override async Task<AddToDoCommand> HandleAsync(AddToDoCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            await using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                var savedItem = await repository.AddAsync(
                    new ToDoItem {Title = command.Title, Completed = command.Completed, Order = command.Order},
                    cancellationToken
                );
                command.ToDoItemId = savedItem.Id;
            }

            _commandProcessor.Post(new TaskCreatedEvent(command.Title));
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}