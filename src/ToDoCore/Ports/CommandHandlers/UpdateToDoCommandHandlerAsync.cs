using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Paramore.Brighter;
using Paramore.Brighter.Logging.Attributes;
using Paramore.Brighter.Policies.Attributes;
using ToDoCore.Adaptors.Db;
using ToDoCore.Adaptors.Repositories;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.CommandHandlers
{
    public class UpdateToDoCommandHandlerAsync : RequestHandlerAsync<UpdateToDoCommand>
    {
        private readonly IAmACommandProcessor _commandProcessor;
        private readonly DbContextOptions<ToDoContext> _options;

        public UpdateToDoCommandHandlerAsync(DbContextOptions<ToDoContext> options,
            IAmACommandProcessor commandProcessor)
        {
            _options = options;
            _commandProcessor = commandProcessor;
        }

        [RequestLoggingAsync(1, HandlerTiming.Before)]
        [UsePolicyAsync(CommandProcessor.CIRCUITBREAKERASYNC, 2)]
        [UsePolicyAsync(CommandProcessor.RETRYPOLICYASYNC, 3)]
        public override async Task<UpdateToDoCommand> HandleAsync(UpdateToDoCommand command,
            CancellationToken cancellationToken = new CancellationToken())
        {
            string title;

            await using (var uow = new ToDoContext(_options))
            {
                var repository = new ToDoItemRepositoryAsync(uow);
                var toDoItem = await repository.GetAsync(command.ToDoId, cancellationToken);

                if (command.Title != null)
                    toDoItem.Title = command.Title;

                if (command.Complete.HasValue)
                    toDoItem.Completed = command.Complete.Value;

                if (command.Order.HasValue)
                    toDoItem.Order = command.Order.Value;

                await repository.UpdateAsync(toDoItem, cancellationToken);

                title = toDoItem.Title;
            }

            if (command.Complete.HasValue && command.Complete.Value)
            {
                var taskCompleteEvent = new TaskCompletedEvent(title);

                _commandProcessor.Post(taskCompleteEvent);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}