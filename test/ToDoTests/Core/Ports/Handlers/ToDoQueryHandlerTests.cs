using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ToDoCore.Adaptors.Db;
using ToDoCore.Model;
using ToDoCore.Ports.Handlers;
using ToDoCore.Ports.Queries;

namespace ToDoTests.Core.Ports.Handlers
{
    [TestFixture]
    public class ToDoQueryHandlerTests
    {
        [Test]
        public async Task Test_Retrieveing_A_Task()
        {
            /*
                Given that I have a database with a ToDo
                When I retrieve that todo
                Then I should get a view model for it

            */
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Retrieving_tasks_from_database")
                .Options;

            var toDoItem = new ToDoItem(){Title = "Make test pass", Completed = false, Order=523};
            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(toDoItem);
                context.SaveChanges();
            }

            var retriever = new ToDoByIdQueryHandlerAsync(options);
            var task = await retriever.ExecuteAsync(new ToDoByIdQuery(toDoItem.Id));

            Assert.AreEqual(toDoItem.Id, task.Id);
            Assert.AreEqual(toDoItem.Title, task.Title);
            Assert.AreEqual(toDoItem.Completed, task.Completed);
            Assert.AreEqual(toDoItem.Order.Value, task.Order.Value);

        }

        [Test]
        [Ignore("Skip and take not working under EF Core")]
        public async Task Test_Retrieving_All_Tasks()
        {
            /*
                Given that I have a database with many ToDos
                When I retrieve a a page of those todos
                Then I should get an interable view model for them

            */
            var options = new DbContextOptionsBuilder<ToDoContext>()
                .UseInMemoryDatabase(databaseName: "Retrieving_tasks_from_database")
                .Options;

            using (var context = new ToDoContext(options))
            {
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.ToDoItems.Add(new ToDoItem(){Title = "Make test pass"});
                context.SaveChanges();
            }

            var retriever = new ToDoQueryAllHandlerAsync(options);
            var request = await retriever.ExecuteAsync(new ToDoQueryAll(1, 3));
            Assert.AreEqual(request.ToDoItems.Count(), 3);
            request = retriever.Execute(new ToDoQueryAll(2, 3));   //only two available on this page
            Assert.AreEqual(request.ToDoItems.Count(), 2);



        }
    }
}