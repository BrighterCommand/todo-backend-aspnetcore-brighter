using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Commands
{
    public class DeleteToDoByIdCommand : Command
    {
        public int ToDoId { get; }

        public DeleteToDoByIdCommand(int toDoId) : base(Guid.NewGuid())
        {
            ToDoId = toDoId;
        }
    }
}
