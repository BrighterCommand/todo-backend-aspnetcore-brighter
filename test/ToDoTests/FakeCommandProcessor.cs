using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoTests
{
    public class FakeCommandProcessor : IAmACommandProcessor

    {
        public bool SentCompletedEvent { get; private set; }
        public bool SentCreatedEvent { get; set; }

        void IAmACommandProcessor.Post<T>(T request)
        {
            if (request.GetType() == typeof(TaskCompletedEvent))
                SentCompletedEvent = true;
            else if (request.GetType() == typeof(TaskCreatedEvent)) SentCreatedEvent = true;
        }

        Task IAmACommandProcessor.PostAsync<T>(T request, bool continueOnCapturedContext,
            CancellationToken cancellationToken)
        {
            if (request.GetType() == typeof(TaskCompletedEvent))
                SentCompletedEvent = true;
            else if (request.GetType() == typeof(TaskCreatedEvent)) SentCreatedEvent = true;
            return Task.CompletedTask;
        }

        public Guid DepositPost<T>(T request) where T : class, IRequest
        {
            throw new NotImplementedException();
        }

        public Task<Guid> DepositPostAsync<T>(T request, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken()) where T : class, IRequest
        {
            throw new NotImplementedException();
        }

        public void ClearOutbox(params Guid[] posts)
        {
            throw new NotImplementedException();
        }

        public void ClearOutbox(int amountToClear = 100, int minimumAge = 5000)
        {
            throw new NotImplementedException();
        }

        public Task ClearOutboxAsync(IEnumerable<Guid> posts, bool continueOnCapturedContext = false,
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public void ClearAsyncOutbox(int amountToClear = 100, int minimumAge = 5000, bool useBulk = false)
        {
            throw new NotImplementedException();
        }

        public TResponse Call<T, TResponse>(T request, int timeOutInMilliseconds)
            where T : class, ICall where TResponse : class, IResponse
        {
            throw new NotImplementedException();
        }

        void IAmACommandProcessor.Publish<T>(T @event)
        {
            throw new NotImplementedException();
        }

        Task IAmACommandProcessor.PublishAsync<T>(T @event, bool continueOnCapturedContext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        void IAmACommandProcessor.Send<T>(T command)
        {
            throw new NotImplementedException();
        }

        Task IAmACommandProcessor.SendAsync<T>(T command, bool continueOnCapturedContext,
            CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}