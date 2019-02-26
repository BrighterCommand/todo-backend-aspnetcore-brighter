using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;

namespace ToDoTests.Core.Ports.Handlers
{
    public class UpdateToDoCommandHandlerTest
    {
        [Test]
        public async Task Test_Updating_a_ToDo_Title()
        {

            /*
                Given that I have a command to update a ToDo's title
                When I handle that command
                Then I should update the ToDo

            */
            const string TODO_TITLE = "test_title";

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "title_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem() { Title = "This title will be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var fakeCommandProcessor = new FakeCommandProcessor();

            var command = new UpdateToDoCommand(toDoItem.Id, title: TODO_TITLE);
            var handler = new UpdateToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(false, context.ToDoItems.Single().Completed);
            }

            // Should not send task complete event
            Assert.IsFalse(fakeCommandProcessor.SentCompletedEvent);
        }

        [Test]
        public async Task Test_Updating_a_ToDo_Completed()
        {

            /*
                Given that I have a command to update a ToDo's complete
                When I handle that command
                Then I should update the ToDo

            */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "completed_writes_to_database")
                .Options;

            var fakeCommandProcessor = new FakeCommandProcessor();

            var toDoItem = new ToDoItem() { Title = "This title won't be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var command = new UpdateToDoCommand(toDoItem.Id, complete: true);
            var handler = new UpdateToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(toDoItem.Title, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
            }

            // Has sent task complete event
            Assert.IsTrue(fakeCommandProcessor.SentCompletedEvent);
        }

        [Test]
        public async Task Test_Updating_a_ToDo_Title_and_Completed()
        {

            /*
                Given that I have a command to pdate add a ToDo's Title and Completed
                When I handle that command
                Then I should update the ToDo

            */

            const string TODO_TITLE = "test_title";

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "titlecompleted_writes_to_database")
                .Options;

            var fakeCommandProcessor = new FakeCommandProcessor();

            var toDoItem = new ToDoItem() { Title = "This title will be changed" };
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, title: TODO_TITLE, complete: true);
            var handler = new UpdateToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(TODO_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
            }

            // Has sent task complete event
            Assert.IsTrue(fakeCommandProcessor.SentCompletedEvent);
        }

        [Test]
        public async Task Test_Updating_The_ToDo_Order()
        {
            /*
                Given that I have a command to update a ToDo's complete
                When I handle that command
                Then I should update the ToDo

            */

            const int NEW_ORDER = 523;

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "order_writes_to_database")
                .Options;
            
            var fakeCommandProcessor = new FakeCommandProcessor();

            var toDoItem = new ToDoItem() { Title = "This title won't be changed", Completed = true, Order = 10};
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, order: NEW_ORDER);
            var handler = new UpdateToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(toDoItem.Title, context.ToDoItems.Single().Title);
                Assert.AreEqual(true, context.ToDoItems.Single().Completed);
                Assert.AreEqual(NEW_ORDER, context.ToDoItems.Single().Order);
            }

            // Should not send task complete event
            Assert.IsFalse(fakeCommandProcessor.SentCompletedEvent);
        }

        [Test]
        public async Task Test_Updating_The_ToDo_Title_Completed_Order()
        {
            /*
                Given that I have a command to update a ToDo's title, complete, & order
                When I handle that command
                Then I should update the ToDo

            */

            const string NEW_TITLE = "New Title";
            const bool NEW_COMPLETED = false;
            const int NEW_ORDER = 523;

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "order_title_completed_writes_to_database")
                .Options;

            var fakeCommandProcessor = new FakeCommandProcessor();

            var toDoItem = new ToDoItem() { Title = "Title", Completed = true, Order = 10};
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }


            var command = new UpdateToDoCommand(toDoItem.Id, title:NEW_TITLE, complete: NEW_COMPLETED, order: NEW_ORDER);
            var handler = new UpdateToDoCommandHandlerAsync(options, fakeCommandProcessor);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(1, context.ToDoItems.Count());
                Assert.AreEqual(NEW_TITLE, context.ToDoItems.Single().Title);
                Assert.AreEqual(NEW_COMPLETED, context.ToDoItems.Single().Completed);
                Assert.AreEqual(NEW_ORDER, context.ToDoItems.Single().Order);
            }
            // Should not send task complete event
            Assert.IsFalse(fakeCommandProcessor.SentCompletedEvent);
        }
    }
}