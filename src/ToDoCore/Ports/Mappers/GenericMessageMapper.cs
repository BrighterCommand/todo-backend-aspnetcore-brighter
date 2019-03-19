using Newtonsoft.Json;
using Paramore.Brighter;

namespace ToDoCore.Ports.Mappers
{
    public class GenericMessageMapper<T> : IAmAMessageMapper<T> where T : class, IRequest
    {
        public Message MapToMessage(T request)
        {
            var messageType = MessageType.MT_EVENT;

            if (request is Event)
                messageType = MessageType.MT_EVENT;


            if (request is Command)
                messageType = MessageType.MT_COMMAND;
            

            var header = new MessageHeader(request.Id, typeof(T).Name, messageType, contentType: "application/json");


            var body = new MessageBody(JsonConvert.SerializeObject(request));
            var message = new Message(header, body);
            return message;
        }

        public T MapToRequest(Message message)
        {
            return JsonConvert.DeserializeObject<T>(message.Body.Value);
        }
    }
}