using Newtonsoft.Json;
using Paramore.Brighter;
using ToDoCore.Ports.Events;

namespace ToDoCore.Ports.Mappers
{
    public class TaskCreatedEventMessageMapper : IAmAMessageMapper<TaskCreatedEvent>
    {
        public Message MapToMessage(TaskCreatedEvent request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "taskcreated.event", messageType: MessageType.MT_EVENT);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public TaskCreatedEvent MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<TaskCreatedEvent>(message.Body.Value);
        }
    }
}