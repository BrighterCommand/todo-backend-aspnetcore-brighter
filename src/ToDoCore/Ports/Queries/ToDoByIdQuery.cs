using Darker;
using ToDoCore.Model;

namespace ToDoCore.Ports.Queries
{
    public class ToDoByIdQuery : IQueryRequest<ToDoByIdQuery.Result>
    {
        public int Id { get; }

        public ToDoByIdQuery(int id)
        {
            Id = id;
        }

        public sealed class Result : IQueryResponse
        {
            public Result(int id, string title)
            {
                Id = id;
                Title = title;
            }

            public Result(ToDoItem toDoItem)
            {
                Id = toDoItem.Id;
                Title = toDoItem.Title;
                Completed = toDoItem.Completed;
                Order = toDoItem.Order;
            }

            public int Id { get; }
            public string Title { get; }
            public bool Completed { get; }
            public int? Order { get; }
            public string Url { get; set; }
        }
    }
}