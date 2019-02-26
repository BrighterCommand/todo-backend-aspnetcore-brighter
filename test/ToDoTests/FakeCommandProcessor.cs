using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoTests
{
    public class FakeCommandProcessor : IAmACommandProcessor

    {
        public FakeCommandProcessor()
        {
        }

        public bool SentCompletedEvent {get; private set;}
        public bool SentCreatedEvent { get; set; }

        void IAmACommandProcessor.Post<T>(T request)
        {
            if(request.GetType() == typeof(TaskCompletedEvent))
            {
                SentCompletedEvent = true;
            }
            else if (request.GetType() == typeof(TaskCreatedEvent))
            {
                SentCreatedEvent = true;
            }
            
        }

        Task IAmACommandProcessor.PostAsync<T>(T request, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            if(request.GetType() == typeof(TaskCompletedEvent))
            {
                SentCompletedEvent = true;
            }
            else if (request.GetType() == typeof(TaskCreatedEvent))
            {
                SentCreatedEvent = true;
            }
            return Task.CompletedTask;
        }

        void IAmACommandProcessor.Publish<T>(T @event)
        {
            throw new System.NotImplementedException();
        }

        Task IAmACommandProcessor.PublishAsync<T>(T @event, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        void IAmACommandProcessor.Send<T>(T command)
        {
            throw new System.NotImplementedException();
        }

        Task IAmACommandProcessor.SendAsync<T>(T command, bool continueOnCapturedContext, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}