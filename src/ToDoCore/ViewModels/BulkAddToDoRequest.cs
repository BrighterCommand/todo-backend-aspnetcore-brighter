using System.Collections.Generic;

namespace ToDoCore.ViewModels
{
    public class BulkAddToDoRequest
    {
        public BulkAddToDoRequest()
        {
            ItemsToAdd = new List<AddToDoRequest>();
        }

        public List<AddToDoRequest> ItemsToAdd { get; set; }
    }
}