namespace ToDoCore.ViewModels
{
    public class AddToDoRequest
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int? Order { get; set; }
    }
}