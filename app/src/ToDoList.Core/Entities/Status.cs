using System.Collections.Generic;

using ToDoList.SharedKernel;

namespace ToDoList.Core.Entities
{
    public class Status : BaseEntity
    {
        public bool IsDone { get; set; }

        public Status() : base()
        {
            TodoItems = new HashSet<TodoItem>();
        }

        public ICollection<TodoItem> TodoItems { get; set; }
    }
}
