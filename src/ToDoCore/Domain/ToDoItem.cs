﻿using ToDoCore.Ports.Repositories;

namespace ToDoCore.Domain
{
    public class ToDoItem : IEntity
    {
        public string Title { get; set; }
        public bool Completed { get; set; }
        public int? Order { get; set; }
        public int Id { get; set; }
    }
}