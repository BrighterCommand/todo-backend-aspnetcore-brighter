using System.Collections.Generic;
using Paramore.Darker;

namespace ToDoCore.Ports.Queries
{
    public class ToDoQueryAll : IQuery<ToDoQueryAll.Result>
    {
        public ToDoQueryAll(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; }
        public int PageSize { get; }

        public sealed class Result
        {
            public Result(IEnumerable<ToDoByIdQuery.Result> items)
            {
                ToDoItems = items;
            }

            public IEnumerable<ToDoByIdQuery.Result> ToDoItems { get; }
        }
    }
}