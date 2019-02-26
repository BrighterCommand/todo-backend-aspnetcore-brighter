using System;
using Paramore.Brighter;

namespace ToDoCore.Ports.Events
{
    public class TaskCreatedEvent : Event
    {
        public TaskCreatedEvent(string title) : base(Guid.NewGuid())
        {
            Title = title;
            Completed = false;
        }
        
        public string Title { get; }
        public bool Completed { get; }
        
    }
}