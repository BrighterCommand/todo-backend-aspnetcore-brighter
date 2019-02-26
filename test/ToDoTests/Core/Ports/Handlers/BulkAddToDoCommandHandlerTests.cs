using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Ports.Commands;
using ToDoCore.Ports.Handlers;
using ToDoCore.ViewModels;

namespace ToDoTests.Core.Ports.Handlers
{
    [TestFixture]
    public class BulkAddToDoCommandHandlerFixture
    {
        [Test]
        public async Task Test_Bulk_Adding_Tasks()
        {
            /*
                Given that I have a list of commands to add a ToDo
                When I handle that command
                Then I should add those to the list of Tasks

            */

            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Bulk_adding_tasks")
                .Options;

            var todos = new List<AddToDoRequest>
            {
                new AddToDoRequest {Title = "First", Order = 1, Completed = false},
                new AddToDoRequest {Title = "Second", Order = 2, Completed = false},
                new AddToDoRequest {Title = "Third", Order = 3, Completed = false}
            };

            var command = new BulkAddToDoCommand(todos) ;
            var handler = new BulkAddToDoCommandHandlerAsync(options);

            await handler.HandleAsync(command);

            using (var context = new ToDoContext(options))
            {
                Assert.AreEqual(3, context.ToDoItems.Count());
                Assert.AreEqual("First", context.ToDoItems.First().Title);
                Assert.AreEqual(false, context.ToDoItems.First().Completed);
                Assert.AreEqual(1,  context.ToDoItems.First().Order.Value);
            }
         }
    }
}