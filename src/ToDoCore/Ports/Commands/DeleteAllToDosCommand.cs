using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Commands
{
    public class DeleteAllToDosCommand : Command
    {
        public DeleteAllToDosCommand() : base(Guid.NewGuid())
        {
        }
    }
}