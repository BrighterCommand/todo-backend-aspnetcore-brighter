
using System;
using  Paramore.Brighter;

namespace ToDoCore.Ports.Commands
{
    public class AddToDoCommand : Command
    {
        public string Title { get; }
        public bool Commpleted { get; }
        public int? Order { get; }

        //out
        public int ToDoItemId { get; set; }

        public AddToDoCommand(string title, bool completed = false, int? order = null)
            : base(Guid.NewGuid())
        {
            Title = title;
            Commpleted = completed;
            Order = order;
        }

    }
}