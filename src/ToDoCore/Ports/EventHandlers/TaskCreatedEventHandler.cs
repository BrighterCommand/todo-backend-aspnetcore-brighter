using System;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.EventHandlers
{
    public class TaskCreatedEventHandler : RequestHandlerAsync<TaskCreatedEvent>
    {
        public override Task<TaskCreatedEvent> HandleAsync(TaskCreatedEvent command, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"Task Create with title \"{command.Title}\"");
            
            return base.HandleAsync(command, cancellationToken);
        }
    }
}