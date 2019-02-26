using System;
using  Paramore.Brighter;

namespace ToDoCore.Ports.Events
{
    public class TaskCompletedEvent : Event
    {
        public string Title { get; private set;}
        public TaskCompletedEvent(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }
    }
}