using Newtonsoft.Json;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.Mappers
{
    public class TaskCompleteEventMessageMapper : IAmAMessageMapper<TaskCompletedEvent>
    {
        public Message MapToMessage(TaskCompletedEvent request)
        {
            var header = new MessageHeader(request.Id, nameof(TaskCompletedEvent), MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public TaskCompletedEvent MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<TaskCompletedEvent>(message.Body.Value);
        }
    }
}