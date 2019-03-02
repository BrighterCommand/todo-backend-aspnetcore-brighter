using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.CommandHandlers;
using ToDoCore.Ports.Commands;

namespace ToDoTests.Core.Ports.CommandHandlers
{
    [TestFixture]
    public class DeleteToDoCommandHandlerTests
    {
        [Test]
        public async Task Delete_All_ToDos()
        {
            /*
               Given that I have ToDos in my database
               When I issue a command to delete all of them
               Then I should remove the ToDos from the database
           */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(new ToDoItem {Title = "Make delete test pass"});
                context.ToDoItems.Add(new ToDoItem {Title = "Make delete test pass"});
                context.SaveChanges();
            }

            var command = new DeleteAllToDosCommand();
            var handler = new DeleteAllToDosCommandHandlerAsync(options);

            await handler.HandleAsync(command);


            using (var context = new ToDoContext(options))
            {
                Assert.IsFalse(context.ToDoItems.Any());
            }
        }

        [Test]
        public async Task Deleting_ToDo_By_Id()
        {
            /*
              Given that I have a ToDo in the database
              When I issue a command to delete the ToDo with that Id
              Then I should remove the ToDo from the database
          */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase("Add_writes_to_database")
                .Options;

            var toDoItem = new ToDoItem {Title = "Make delete test pass"};
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var command = new DeleteToDoByIdCommand(toDoItem.Id);
            var handler = new DeleteToDoByIdCommandHandlerAsync(options);

            await handler.HandleAsync(command);


            using (var context = new ToDoContext(options))
            {
                Assert.IsFalse(context.ToDoItems.Any(t => t.Id == toDoItem.Id));
            }
        }
    }
}