using Newtonsoft.Json;
using Paramore.Brighter;
using ToDoCore.Ports.Commands;

namespace ToDoCore.Ports.Mappers
{
    public class BulkAddToDoMessageMapper : IAmAMessageMapper<BulkAddToDoCommand>
    {
        public Message MapToMessage(BulkAddToDoCommand request)
        {
            var header = new MessageHeader(messageId: request.Id, topic: "bulkaddtodo.command", messageType: MessageType.MT_COMMAND);
            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public BulkAddToDoCommand MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<BulkAddToDoCommand>(message.Body.Value);
        }
    }
}