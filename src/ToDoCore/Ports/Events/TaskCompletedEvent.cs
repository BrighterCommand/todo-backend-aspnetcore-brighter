using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Events
{
    public class TaskCompletedEvent : Event
    {
        public TaskCompletedEvent(string title) : base(Guid.NewGuid())
        {
            Title = title;
        }

        public string Title { get; }
    }
}