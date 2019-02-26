using System.Collections.Generic;
using Darker;

namespace ToDoCore.Ports.Queries
{
    public class ToDoQueryAll : IQueryRequest<ToDoQueryAll.Result>
    {
        public ToDoQueryAll(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; }
        public int PageSize { get; }

        public sealed class Result : IQueryResponse
        {
            public IEnumerable<ToDoByIdQuery.Result> ToDoItems { get; }

            public Result(IEnumerable<ToDoByIdQuery.Result> items)
            {
                ToDoItems = items;
            }
        }
    }
}