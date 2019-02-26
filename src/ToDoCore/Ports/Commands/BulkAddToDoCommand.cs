using System;
using System.Collections.Generic;
using Paramore.Brighter;
using ToDoCore.ViewModels;

namespace ToDoCore.Ports.Commands
{
    public class BulkAddToDoCommand : Command
    {
        public IEnumerable<AddToDoRequest> ToDos { get; set; }
        public IList<int> ToDoItemIds { get; set; } //out parameter

        public BulkAddToDoCommand()
            :base(Guid.NewGuid())
        {
            ToDos = new List<AddToDoRequest>();
            ToDoItemIds = new List<int>();
        }

        public BulkAddToDoCommand(IEnumerable<AddToDoRequest> todos)
            : base(Guid.NewGuid())
        {
            ToDos = todos;
            ToDoItemIds = new List<int>();
        }
    }
}