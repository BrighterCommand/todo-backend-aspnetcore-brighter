using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Commands
{
    public class DeleteToDoByIdCommand : Command
    {
        public DeleteToDoByIdCommand(int toDoId) : base(Guid.NewGuid())
        {
            ToDoId = toDoId;
        }

        public int ToDoId { get; }
    }
}