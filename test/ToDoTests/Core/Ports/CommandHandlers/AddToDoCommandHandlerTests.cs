using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.CommandHandlers;
using ToDoCore.Ports.Commands;

namespace ToDoTests.Core.Ports.CommandHandlers
{
    [TestFixture]
    public class AddToDoCommandHandlerFixture
    {
        [Test]
        public async Task Test_Adding_A_Task()
        {
            /*
                Given that I have a command to add a ToDo
                When I handle that command
                Then I should add to the list of Tasks

            */
            const string TODO_TITLE = "test_title";
            const int ORDER_NUM = 10;

            var fakeCommandProcessor = new FakeCommandProcessor();

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            var command = new AddToDoCommand(TODO_TITLE, true, ORDER_NUM);
            var handler = new AddToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
                Assert.AreEqual(ORDER_NUM, context.ToDoItems.Single().Order.Value);
            }

            Assert.IsTrue(fakeCommandProcessor.SentCreatedEvent);
        }
    }
}