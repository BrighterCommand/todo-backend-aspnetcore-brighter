using System;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.EventHandlers
{
    public class TaskCompletedEventHandler : RequestHandlerAsync<TaskCompletedEvent>
    {
        public override Task<TaskCompletedEvent> HandleAsync(TaskCompletedEvent command, CancellationToken cancellationToken = new CancellationToken())
        {
            Console.WriteLine($"Task with title \"{command.Title}\" has been completed");
            
            return base.HandleAsync(command, cancellationToken);
        }
    }
}