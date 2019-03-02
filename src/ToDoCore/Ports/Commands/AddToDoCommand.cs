using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public AddToDoCommand(string title, bool completed = false, int? order = null)
            : base(Guid.NewGuid())
        {
            Title = title;
            Completed = completed;
            Order = order;
        }

        public string Title { get; }
        public bool Completed { get; }
        public int? Order { get; }

        //out
        public int ToDoItemId { get; set; }
    }
}