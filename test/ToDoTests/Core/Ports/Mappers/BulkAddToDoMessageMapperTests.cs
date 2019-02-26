using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using NUnit.Framework;
using Paramore.Brighter;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Mappers;
using ToDoCore.ViewModels;

namespace ToDoTests.Core.Ports.Mappers
{
    [TestFixture]
    public class BulkAddToDoMessageMapperTests
    {
        [Test]
        public void Test_Map_A_Bulk_Command_To_A_Message()
        {
            /* Given that I have a bulk command
               When I map it to a message
               I should get a message header and a JSON body
            */

            var todos = new List<AddToDoRequest>
            {
                new AddToDoRequest {Title = "First", Order = 1, Completed = false},
                new AddToDoRequest {Title = "Second", Order = 2, Completed = false},
                new AddToDoRequest {Title = "Third", Order = 3, Completed = false}
            };

            var command = new BulkAddToDoCommand(todos) ;

            var message = new BulkAddToDoMessageMapper().MapToMessage(command);

            Assert.AreEqual(message.Header.MessageType, MessageType.MT_COMMAND);
            Assert.AreEqual(message.Header.Topic, "bulkaddtodo.command");
            Assert.AreEqual(message.Body.Value, JsonConvert.SerializeObject(command));

        }

        [Test]
        public void Test_Map_A_Message_To_A_Bulk_Action_Command()
        {
            /* Given that I have a message for a bulk command
               When I map it to a command
               I should get a command with a list of bulk options
            */

            var todos = new List<AddToDoRequest>
            {
                new AddToDoRequest {Title = "First", Order = 1, Completed = false},
                new AddToDoRequest {Title = "Second", Order = 2, Completed = false},
                new AddToDoRequest {Title = "Third", Order = 3, Completed = false}
            };

            var original_command = new BulkAddToDoCommand(todos) ;

            var header = new MessageHeader(messageId: Guid.NewGuid(), topic: "bulkaddtodo.command", messageType: MessageType.MT_COMMAND);
            var body = new MessageBody(JsonConvert.SerializeObject(original_command));
            var message = new Message(header, body);

            var command = new BulkAddToDoMessageMapper().MapToRequest(message);

            Assert.AreEqual(command.ToDos.Count(), 3);
            Assert.AreEqual(command.ToDos.First().Title, "First");
            Assert.AreEqual(command.ToDos.First().Order, 1);
            Assert.AreEqual(command.ToDos.First().Completed, false);
        }
   }
}